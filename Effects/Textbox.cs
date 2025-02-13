using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public partial class Textbox : CanvasLayer
{
    public bool readingInProgress;

	// things in the scene
	Label startSymbol;
    Label labelString;
    Label endSymbol;
	MarginContainer textBoxContainer;

	TextboxStates state;
	bool readingDone;

	Queue<string> textQueue;

	Tween textDisplayTween;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		startSymbol = GetNode<Label>("TextBoxContainer/MarginContainer/HBoxContainer/StartSymbol");
		labelString = GetNode<Label>("TextBoxContainer/MarginContainer/HBoxContainer/Label");
		endSymbol = GetNode<Label>("TextBoxContainer/MarginContainer/HBoxContainer/EndSymbol");
		textBoxContainer = GetNode<MarginContainer>("TextBoxContainer");

        state = TextboxStates.READY;
		HideTextbox();

		textQueue = new Queue<string>();
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        // TODO: do I want to pause the game or something here?
        readingInProgress = textBoxContainer.Visible;

        runStateMachine(delta);
    }

	private void runStateMachine(double delta)
	{
        switch (state)
        {
            case TextboxStates.READY:
                if (textQueue.Count > 0)
                {
                    ShowTextbox();
                    textDisplayTween = displayText();
                    readingDone = false;
                    state = TextboxStates.READING;
                }
                else
                {
                    HideTextbox();
                }
                break;
            case TextboxStates.READING:
                if (Input.IsActionJustPressed("enter"))
                {
                    // finish tween immediately
                    textDisplayTween.CustomStep(99);
                }
                if (readingDone)
                    state = TextboxStates.FINISHED;
                break;
            case TextboxStates.FINISHED:
                endSymbol.Text = "v";
                if (Input.IsActionJustPressed("enter"))
                {
                    endSymbol.Text = "";
                    state = TextboxStates.READY;
                }
                break;
        }
    }

	public void AddText(string textToAdd)
	{
        Debug.Assert(textToAdd is not null, "Text not set for textbox");
		textQueue.Enqueue(textToAdd);
    }

	private Tween displayText()
	{
        //labelString.Text = textToAdd;
        ShowTextbox();
		labelString.Text = textQueue.Dequeue();
        labelString.VisibleRatio = 0;
        var charCount = labelString.Text.Length;

        // show text
        double charsPerSecond = 15;
		var tween = GetTree().CreateTween();
        tween.TweenProperty(labelString, "visible_ratio", 1, charCount / charsPerSecond);

        // connect signal
        var myCallable = Callable.From(OnFinished);
        tween.Connect("finished", myCallable);

		return tween;
    }

    private void HideTextbox()
	{
		startSymbol.Text = "";
		labelString.Text = "";
		endSymbol.Text = "";
		textBoxContainer.Visible = false;
    }

	private void ShowTextbox()
	{
        startSymbol.Text = "*";
        textBoxContainer.Visible = true;

		// TODO: use visible ratio on label string, to make chars appear one at at time, undertale style bby
    }

	private void OnFinished()
	{
		readingDone = true;
        endSymbol.Text = "v";
    }
}
