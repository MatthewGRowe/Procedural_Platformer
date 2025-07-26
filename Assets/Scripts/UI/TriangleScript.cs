using UnityEngine;
using UnityEngine.UI; // Needed to work with Buttons
using TMPro;           // Needed to work with TextMeshProUGUI

public class TriangleScript: MonoBehaviour
{
    public TextMeshProUGUI targetLetterText; // This is the letter text (A, B, C...) under this triangle
    public bool isUpButton = true;           // Is this an UP button? (If false, it's a DOWN button)

    private void Start()
    {
        // Get the Button component on this GameObject (this triangle)
        Button button = GetComponent<Button>();
        
        if (button != null)
        {
            // Tell Unity: "Hey, when this button gets clicked, run the OnClick() method below"
            button.onClick.AddListener(OnClick);
        }
        else
        {
            Debug.LogWarning("No Button component found on " + gameObject.name);
        }
        
    }

    private void OnClick()
    {
        // This method runs when the triangle is clicked
        if (targetLetterText == null)
        {
            Debug.LogWarning("No letter text linked to this triangle!");
            return;
        }

        // Get the current letter from the text
        char currentChar = targetLetterText.text.Length > 0 ? targetLetterText.text[0] : ' ';

        /*^^^^^^ THIS IS A FANCY WAY OF WRITING AN IF STATEMENT ^^^^^^^^
         *        vvvvvv  BASICALLY IT IS THIS  vvvvvvv
                char currentChar;

                if (targetLetterText.text.Length > 0)
                {
                    currentChar = targetLetterText.text[0];
                }
                else
                {
                    currentChar = ' '; // space character
                }
        /*********  IT'S CALLED A TERNARY OPERATOR! ****************/



        // Figure out what the new letter should be
        char newChar = GetNextLetter(currentChar, isUpButton);

        // Update the text to show the new letter
        targetLetterText.text = newChar.ToString();
    }

    // This method picks the next letter, up or down
    private char GetNextLetter(char current, bool goingUp)
    {
        string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ ";
        int index = alphabet.IndexOf(char.ToUpper(current));

        // If somehow the current character is NOT found, default to A
        if (index == -1)
            index = 0;

        if (goingUp)
        {
            index = (index + 1) % alphabet.Length; // Move forward, wrap around
        }
        else
        {
            //Uses MOD to calculate the next letter
            index = (index - 1 + alphabet.Length) % alphabet.Length; // Move backward, wrap around
        }

        return alphabet[index];
    }
}
