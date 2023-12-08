import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { NoteService } from '../_services/note.service';

@Component({
  selector: 'app-popup',
  templateUrl: './popup.component.html',
  styleUrls: ['./popup.component.css']
})
export class PopupComponent implements OnInit{
  inputData: any;
  editData: any;
  noteId: number = 0;
  popupForm: FormGroup = new FormGroup({});
  constructor(@Inject(MAT_DIALOG_DATA) public data: any, private noteService: NoteService,
  private dialogRef: MatDialogRef<PopupComponent>, private fb: FormBuilder) {}

  ngOnInit(): void {
    this.inputData = this.data;
    this.initializeForm();

    if(this.inputData.id > 0) {
      this.setPopupData(this.inputData.id);
    }
  }

  addNote() {
    this.noteService.addNote(this.popupForm.value).subscribe({
      next: () =>
      {
        this.closePopup()
      }
    })
  }

  updateNote() {
    const model = this.popupForm.value;
    model.id = this.noteId;
    console.log(model)
    this.noteService.updateNote(model).subscribe({
      next: () => {
        this.closePopup();
      }
    })
  }

  setPopupData(id: number)
  {
    this.noteService.getNote(id).subscribe({
      next: response => {
        this.editData = response;
        this.noteId = this.editData.id;
        this.popupForm.setValue({title:this.editData.title, description:this.editData.description})
      }
    });
  }

  closePopup() {
    this.dialogRef.close('Closed using function');
  }

  initializeForm() {
    this.popupForm = this.fb.group({
      title: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(16)]],
      description: ['', Validators.required]
    });
  }
}
