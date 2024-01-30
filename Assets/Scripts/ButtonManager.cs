using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Random = System.Random;
using System.Linq;
using System.Diagnostics;
using System;
using System.Text;
using System.Runtime.InteropServices;
using UnityEngine.SceneManagement;

public class ButtonController : MonoBehaviour
{
    public Button clear_Button, next_Button, userInfo_Button, tryAgain_Button, endGame_Button;
    public LineRenderer line;

    //survey links here
    string between_rounds_prefilled_link = "https://docs.google.com/forms/d/e/1FAIpQLSefdq32iFuIRoZZ24_jjx0-IPZsaJgWeG1KBqMOO5msxE7lvQ/viewform?usp=pp_url&entry.2079787612=dummyID&entry.2043882010=";
    string pre_activity_link = "https://docs.google.com/forms/d/e/1FAIpQLSctMl_OmBSnl_w--_f5FIrYFgAKuAhp-Nu9MZOPBTF2yDIybQ/viewform?usp=pp_url&entry.1434915648=dummyID";
    string post_activity_link = "https://docs.google.com/forms/d/e/1FAIpQLSfDcwD4QIup6mSbLvd6W1tHSfVKbMXiJOjylCpyNhSxW1ffSg/viewform?usp=pp_url&entry.1981905902=dummyID";

    private GameObject img;
    private Image curr_Img;
    private int round = 0;

    private GameObject blockerPanel;
    private GameObject invalidPanel;

    private string group = "opt";

    private string userID = "-1";

    private int grid_size;
    private Vector2 goal_loc = new Vector2(-1,-1);
    private Vector2 tail_loc = new Vector2(-1,-1);

    private string final_text = "";

    float maxX = Mathf.NegativeInfinity;
    float maxY = Mathf.NegativeInfinity;
    float minX = Mathf.Infinity;

    GameObject goalText;
    GameObject scoreText;

    int score = 0;
    int scoreTotal = 0;

    private List<Vector2> correctTraj = new List<Vector2>();

    int roundCap = 10;

    DateTime roundStart, roundEnd;

    List<string> envList;

    string e_name = "";
    
    void Start()
    {
        AddListeners();

        SetVariables();

        setUserText();
    }

    void AddListeners(){
        clear_Button.onClick.AddListener(ClearClick);

        next_Button.onClick.AddListener(NextClick);

        userInfo_Button.onClick.AddListener(ProceedClick);

        tryAgain_Button.onClick.AddListener(ClearPopUp);

        endGame_Button.onClick.AddListener(FinishGame);
    }

    void SetVariables(){
        Random rnd = new Random();

        GameObject parent = GameObject.Find("Canvas").transform.Find("MainBackground").gameObject.transform.Find("MapPanel").gameObject;

        img = parent.transform.Find("Image").gameObject;

        curr_Img = img.GetComponent<Image>();

        goalText = GameObject.Find("Canvas").transform.Find("MainBackground").gameObject.transform.Find("MapPanel").transform.Find("GoalText").gameObject;
        scoreText = GameObject.Find("Canvas").transform.Find("MainBackground").gameObject.transform.Find("LeftPanel").gameObject.transform.Find("ScoreText").gameObject;

        invalidPanel = GameObject.Find("Canvas").transform.Find("InvalidLinePanel").gameObject;
        blockerPanel = GameObject.Find("Canvas").transform.Find("BlockerPanel").gameObject;

        // get list of all enviros poss
        List<string> allPoss = new List<string>();
        string [] fileEntries = Directory.GetFiles("Assets/imgs/" + group +"/");
        foreach(string fileName in fileEntries)
            allPoss.Add(fileName.Split("_")[0].Split(group+"/")[1]);
        //select appropriate random amount of them
        envList = allPoss.OrderBy(x => rnd.Next()).Take(roundCap).ToList();
        //show us/assign
        /*
        foreach(string e in envList){
            print(e);
        }*/
    }

    void ClearLine(){
        line.positionCount = 0;
    }

    void ClearClick()
    {
        //Debug.Log("You have clicked the clear button!");
        ClearLine();
    }

    void ClearPopUp()
    {
        invalidPanel.SetActive(false);
        ClearLine();
        goalText.SetActive(true);
        if (score != 0){
            scoreText.SetActive(true);
        }
    }

    void SetGoalLoc(string text){
        //get x
        string goal = text.Split("|")[1];
        goal_loc.x = int.Parse(goal.Split(",")[0].Substring(1));
        //get y
        goal_loc.y = int.Parse(goal.Split(" ")[1].Split("]")[0]);
    }

    void SetTailLoc(string text){
        string tail = text.Split("|")[2];
        tail_loc.x = int.Parse(tail.Split(",")[0].Substring(1));
        tail_loc.y = int.Parse(tail.Split(" ")[1].Split("]")[0]);
    }

    void UpdateMinMax(){
        var bound = new Vector3[4];
        curr_Img.rectTransform.GetWorldCorners(bound);

        // update min and max
        foreach (var vector3 in bound)
        {
            //Debug.Log(vector3);
            if(vector3.x > maxX){
                maxX = vector3.x;
            }
            if(vector3.y > maxY){
                maxY = vector3.y;
            }
            if(vector3.x < minX){
                minX = vector3.x;
            }
        }
    }

    void UpdateCorrectTraj(){
        //print(e_name);
        string filePath = "Assets/Resources/trajs/env_" + e_name.Split("env")[1] + "_" + group + ".csv";
        StreamReader reader = null;
        correctTraj.Clear();
        if (File.Exists(filePath)){
            reader = new StreamReader(File.OpenRead(filePath));
            while (!reader.EndOfStream){
                var line = reader.ReadLine();
                var values = line.Split(',');
                int xCor = int.Parse(values[0]);
                int yCor = int.Parse(values[1]);
                correctTraj.Add(new Vector2(xCor,yCor));
            }
        }
        else{
            print("BAD FILE");
        }
    }

    void ResetScores(){
        score = 0;
        scoreText.SetActive(false);
        scoreTotal = 0;
        final_text = "";
    }

    void UpdateEnvValues(){
        string nextEnvTXT = e_name + "_1_" + group;

        string text = File.ReadAllText("Assets/Resources/txts/"+nextEnvTXT+".txt");

        grid_size = int.Parse(text.Split("|")[0]);

        SetGoalLoc(text);
        SetTailLoc(text);

        UpdateMinMax();
    }
    
    void ProceedClick()
    {
        //print(round);
        blockerPanel.SetActive(false);

        ClearLine();

        if(round == 1){
            //pre-survey link
            Application.OpenURL(pre_activity_link.Replace("dummyID",userID));
        }
        else{
            //between rounds link
            GUIUtility.systemCopyBuffer = EncodeText(final_text);
            Application.OpenURL(between_rounds_prefilled_link.Replace("dummyID",userID) + (round - 1).ToString());
        }

        ResetScores();

        if (round > roundCap){
            EndGame();
        }
    }

    void EndGame(){
        //pop up the end screen
        GameObject finalPanel = invalidPanel = GameObject.Find("Canvas").transform.Find("EndPanel").gameObject;
        finalPanel.SetActive(true);

    }

    void FinishGame(){
        Application.OpenURL(post_activity_link.Replace("dummyID",userID));
        Application.Quit();
    }

    void MakeUserID(){
        Random g = new Random();
        userID = g.Next(0, 1000000).ToString("D6");
    }

    void Round1SetUp(){
        //set up behind the panel
        round = 1;
        //reset text
        TMPro.TextMeshProUGUI textMesh = GameObject.Find("Canvas").transform.Find("MainBackground").gameObject.transform.Find("HeaderRow").gameObject.transform.Find("RoundText").GetComponent<TMPro.TextMeshProUGUI>();
        textMesh.text = "Round " + round.ToString();
    }
    void setUserText()
    {
        if( userID == "-1"){
            MakeUserID();
        }
        //then alter text
        TMPro.TextMeshProUGUI idTextMesh = blockerPanel.transform.Find("UserInfoButton").gameObject.transform.Find("IDText").GetComponent<TMPro.TextMeshProUGUI>();
        if (score == -1){
            idTextMesh.text = "User ID: " + userID;
        }
        else{
            idTextMesh.text = "User ID: " + userID + "\n Score: " + score;
        }

        if(round == 0){
            Round1SetUp();
        }

    }

    Vector2 ScalePoint(Vector3 v){
        float scaledX = (float)((v.x - minX) * grid_size) / (maxX - minX);
        float scaledY = (float)(v.y * grid_size) / maxY;
        scaledY = grid_size - scaledY;

        Vector2 scaled_pt = new Vector2(scaledY, scaledX);

        return scaled_pt;
    }

    Vector3 GoalPointToWorld(Vector2 g){
        float xShift = (grid_size * 1.65f) / 14f;
        float scaledX = (((g.y + xShift) * (maxX - minX)) / grid_size) + minX;

        float yShift = (grid_size * 0.4f) / 14f;
        float scaledY = ((grid_size - (g.x + yShift)) * maxY) / grid_size;

        Vector3 scaled_pt = new Vector3(scaledX, scaledY, 0);

        return scaled_pt;
    }

    void NewEnvSetUp(){
        e_name = envList[round - 1];
        string next_img = e_name + "_1_" + group;
        print(next_img);
        Sprite new_sprite = Resources.Load<Sprite>("imgs/" + group + "/" +next_img.Split(".")[0]);
        line.enabled = true;
        ClearLine();

        img.GetComponent<Image>().sprite = new_sprite;

        UpdateEnvValues();
        //set goalText location appropriately
        goalText.transform.position = GoalPointToWorld(goal_loc);

        //update correct trajectory
        UpdateCorrectTraj();

        goalText.SetActive(true);
    }

    string GetNextImg(){
        bool next = false;
        string next_img = "";
        DirectoryInfo dir = new DirectoryInfo("Assets/imgs/"+group);
        FileInfo[] info = dir.GetFiles("*.png");

        //print(curr_Img.sprite.name);

        foreach (FileInfo f in info) 
            { 
                //Debug.Log(f);
                if(next){
                    next_img = f.Name;
                }

                if(f.Name.Contains(curr_Img.sprite.name)){
                    next = true;
                }
                else{
                    next = false;
                }
            }

        return next_img;
    }
    
    Vector3[] HarvestLinePoints(){
        Vector3[] allPoints = new Vector3[line.positionCount];

        line.GetPositions(allPoints);

        return allPoints;
    }

    List<Vector2> ScaleAllPoints(Vector3[] allPoints){
        List<Vector2> allPointsScaled = new List<Vector2>();

        foreach(Vector3 v in allPoints){

            Vector2 scaled_pt = ScalePoint(v);
            allPointsScaled.Add(scaled_pt);
        }

        return allPointsScaled;
    }
    
    bool TailCheck(){
        bool tailCheck = true;
        //first point of line
        Vector2 modded_tail_loc = new Vector2(tail_loc.x + 0.5f, tail_loc.y + 0.5f);
        Vector2 scaled_start = ScalePoint(line.GetPosition(0));

        float upper_x = modded_tail_loc.x + 0.5f;
        float lower_x = modded_tail_loc.x - 0.5f;
        float upper_y = modded_tail_loc.y + 0.5f;
        float lower_y = modded_tail_loc.y - 0.5f;

        if(scaled_start.x <= upper_x && scaled_start.x >= lower_x){
            //Debug.Log("x okay");
        }
        else{
            //Debug.Log("BAD X");
            tailCheck = false;
        }
        if(scaled_start.y <= upper_y && scaled_start.y >= lower_y){
            //Debug.Log("y okay");
        }
        else{
            //Debug.Log("BAD Y");
            tailCheck = false;
        }

        return tailCheck;
    }

    bool GoalCheck(){
        bool goalCheck = true;
        //last point of line
        Vector2 modded_goal_loc = new Vector2(goal_loc.x + 0.5f, goal_loc.y + 0.5f);
        Vector2 scaled_end = ScalePoint(line.GetPosition(line.positionCount - 1));

        float buffer = 0.75f;
        float upper_x = modded_goal_loc.x + buffer;
        float lower_x = modded_goal_loc.x - buffer;
        float upper_y = modded_goal_loc.y + buffer;
        float lower_y = modded_goal_loc.y - buffer;

        if(scaled_end.x <= upper_x && scaled_end.x >= lower_x){
            //Debug.Log("x okay");
        }
        else{
            goalCheck = false;

        }
        if(scaled_end.y <= upper_y && scaled_end.y >= lower_y){
            //Debug.Log("y okay");
        }
        else{
            goalCheck = false;
        }

        return goalCheck;
    }
    
    bool ValidLineCheck(){
        bool tailValid = TailCheck();
        bool goalValid = GoalCheck();

        return tailValid && goalValid;
    }
    
    void BlockerPanelUp(){
        blockerPanel.SetActive(true);
        goalText.SetActive(false);
        scoreText.SetActive(false);
    }
    
    void UpdateRoundText(){
        TMPro.TextMeshProUGUI textMesh = GameObject.Find("Canvas").transform.Find("MainBackground").gameObject.transform.Find("HeaderRow").gameObject.transform.Find("RoundText").GetComponent<TMPro.TextMeshProUGUI>();
        textMesh.text = "Round " + round.ToString();
    }
    
    Sprite NextRoundSetUp(){
        round += 1;
        //Debug.Log("get next round card!");
        Sprite new_sprite = Resources.Load<Sprite>("imgs/round"+round.ToString());
        //disable line
        line.enabled = false;
        //reset text
        UpdateRoundText();

        //make the panel appear
        setUserText();

        BlockerPanelUp();

        return new_sprite;
    }

    Sprite NextImgSetUp(string next_img){
        //Debug.Log(next_img);
        Sprite new_sprite = Resources.Load<Sprite>("imgs/" + group + "/"+next_img.Split(".")[0]);
        //is this the last img???
        if(next_img.Split("_")[1][0] - '0' != 7){
            //enable line
            line.enabled = true;
            goalText.SetActive(true);
        }
        else{
            line.enabled = false;
            goalText.SetActive(false);
        }

        return new_sprite;
    }

    List<float> GetScoreList(List<Vector2> allPointsScaled){
        //get proper traj
        //clip proper traj
        int startIndex = correctTraj.IndexOf(tail_loc);
        List<Vector2> clippedCorrectTraj = correctTraj.GetRange(startIndex,correctTraj.Count - startIndex);
        //bucket user traj
        List<float> scoreList = new List<float>();
        for (int i = 0; i < clippedCorrectTraj.Count; i++){
            int newIndex = (int) ((i * allPointsScaled.Count) / clippedCorrectTraj.Count);
            scoreList.Add(Vector2.Distance(clippedCorrectTraj[i], allPointsScaled[newIndex]));
        } 

        return scoreList; 
    }
    
    void CalculateScore(List<float> scoreList){
        score += (int) ((1000f - scoreList.Sum()) / 1000 * 100);
        scoreTotal += 100;
    }
    
    string EncodeText(string inputText){
        byte[] bytesToEncode = Encoding.UTF8.GetBytes(inputText);
        string encodedText = Convert.ToBase64String(bytesToEncode);

        return encodedText;
    }
    

    Sprite GetNewSprite(string next_name, string next_img){
        Sprite new_sprite;
        if(string.Equals(next_name, e_name)){
            new_sprite = NextImgSetUp(next_img);
        }
        else{
            new_sprite = NextRoundSetUp();
        }

        return new_sprite;
    }
    bool CanUserDraw(string next_img){
        if(curr_Img.sprite.name.Split("_")[1][0] - '0' == 7 || next_img.Split("_")[1][0] - '0' == 7){
            return false;
        }
        else{
            return true;
        }
    }
    void NextClick()
    {
        if(curr_Img.sprite.name.Contains("round")){
            NewEnvSetUp();
            roundStart = DateTime.Now;

        }
        else{
            Sprite new_sprite;
            string next_img = GetNextImg();
            //print(next_img);

            Vector3[] allPoints = HarvestLinePoints();
            List<Vector2> allPointsScaled = ScaleAllPoints(allPoints);

            string pts = string.Join(" ", allPointsScaled.ToArray());
            

            string result = userID+"|"+round.ToString()+"|"+e_name+"|"+pts+"|";

            string tmp = next_img.Split("_")[0];

            new_sprite = GetNewSprite(tmp, next_img);

            bool isValidLine = true;

            isValidLine = ValidLineCheck();

            bool canDraw = CanUserDraw(next_img);
            

            if (isValidLine){
                roundEnd = DateTime.Now;
                List<float> scoreList = GetScoreList(allPointsScaled);
                //calculate score
                CalculateScore(scoreList);
                //add score to result
                result += string.Join(",", scoreList.ToArray()) + "|" + (roundEnd - roundStart).TotalMilliseconds.ToString() + "\n";
                final_text += result;
                // copy to clipboard
                //GUIUtility.systemCopyBuffer = final_text;

                //string encodedText = EncodeText(final_text);

                //print(encodedText);
                
                ClearLine();

                img.GetComponent<Image>().sprite = new_sprite;

                string text = File.ReadAllText("Assets/Resources/txts/"+new_sprite.name+".txt");

                SetGoalLoc(text);

                SetTailLoc(text);

                scoreText.GetComponent<TMPro.TextMeshProUGUI>().text = "Score: " + score.ToString() + " / " + scoreTotal.ToString();
                scoreText.SetActive(true);

                roundStart = DateTime.Now;
            }
            else if (!canDraw){
                //print(new_sprite.name);
                img.GetComponent<Image>().sprite = new_sprite;
            }
            else{
                //get the pop up and enable it!!
                ClearLine();
                invalidPanel.SetActive(true);
            }
        }
    }
}
