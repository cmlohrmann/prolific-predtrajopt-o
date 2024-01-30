using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class NextButtonController : MonoBehaviour
{
    private Image curr_Img;
    private int curr_env;

    void Start()
    {
        GameObject parent = transform.parent.gameObject.transform.parent.gameObject;

        GameObject img = parent.transform.Find("MapPanel").gameObject.transform.Find("Image").gameObject;

        curr_Img = img.GetComponent<Image>();

        //Debug.Log(curr_Img.sprite.name);

        string tmp = curr_Img.sprite.name.Split("_")[0];

        curr_env = tmp[tmp.Length - 1] - '0';

    }

    public void onButtonClick()
    {
        Debug.Log("Clicked button ");
        //get all in same folder in order
        DirectoryInfo dir = new DirectoryInfo("Assets/imgs/opt");
        FileInfo[] info = dir.GetFiles("*.png");
        bool next = false;
        string next_img = "";
        foreach (FileInfo f in info) 
        { 
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

        Debug.Log(next_img);

        //private next_Image = Resources.Load<Image>("Assets/imgs/opt/"+next_img);
    }

    /*
    private void addNewTerm()
    {
        // set the button text to the term name
        if (inputField.text == "")
        {
            System.Console.WriteLine("Input field is empty");
            return;
        }

        // add the new term
        System.Console.WriteLine("Adding new term: " + inputField.text);
        this.termName = inputField.text;
        this.isNewTerm = false;

        buttonText.text = this.termName;

        // print termName and buttonTExt to console
        System.Console.WriteLine("termName: " + this.termName);
        System.Console.WriteLine("buttonText: " + buttonText.text);

        // clear the input field
        inputField.text = "";

        // print termName and buttonTExt to console
        System.Console.WriteLine("termName: " + this.termName);
        System.Console.WriteLine("buttonText: " + buttonText.text);

        // get the HeaderRow object and print its name to console
        GameObject headerRow = this.transform.parent.gameObject;
        System.Console.WriteLine("headerRow: " + headerRow.name);

        // Create a new instance of the TermButton prefab and add it to the HeaderRow
        // New button should be 255 pixels to the right of the current button
        // New button should be 0 pixels above the current button
        GameObject newTermButton = Instantiate(Resources.Load("Prefabs/TermButtonPrefab")) as GameObject; // Could be more efficient if I don't load the resource by name
        newTermButton.transform.SetParent(headerRow.transform, false);
        newTermButton.transform.position = new Vector3(this.transform.position.x + 185, this.transform.position.y, this.transform.position.z);
        

        // set the button controller sciprt propoerty on the new term button 
        // to be the same as the current button
        newTermButton.GetComponent<ButtonController>().inputField = this.inputField;

        // set the onClick method of the new button to be its ButtonController's onButtonClick method
        newTermButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(newTermButton.GetComponent<ButtonController>().onButtonClick);
    }

    private void openExistingTerm()
    {
        System.Console.WriteLine("Opening term: " + this.termName);
    }
    */



}
