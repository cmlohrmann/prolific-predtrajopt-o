using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearButtonController : MonoBehaviour
{
    public LineRenderer line;

    void Start()
    {
    }

    public void onButtonClick()
    {
        Debug.Log("Clicked button ");
        line.positionCount = 0;
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
