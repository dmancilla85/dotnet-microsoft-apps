﻿using System;
using System.IO;
using Xamarin.Forms;

namespace XamarinNotes.Views
{
  public partial class NotesPage : ContentPage
  {
    private readonly string _fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "notes.txt");

    public NotesPage()
    {
      InitializeComponent();

      // Read the file.
      if (File.Exists(_fileName))
      {
        editor.Text = File.ReadAllText(_fileName);
      }
    }

    private void OnSaveButtonClicked(object sender, EventArgs e)
    {
      // Save the file.
      File.WriteAllText(_fileName, editor.Text);
    }

    private void OnDeleteButtonClicked(object sender, EventArgs e)
    {
      // Delete the file.
      if (File.Exists(_fileName))
      {
        File.Delete(_fileName);
      }
      editor.Text = string.Empty;
    }
  }
}