import { Component, OnInit, Output } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { Note } from '../_modules/note';
import { NoteService } from '../_services/note.service';
import { MatDialog } from '@angular/material/dialog';
import { PopupComponent } from '../popup/popup.component';
import { take } from 'rxjs';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit{
  loginMode = false;
  notes: Note[] = [];

  constructor(public accountService: AccountService, private dialog: MatDialog,
    private noteService: NoteService) {}
  
  ngOnInit(): void {
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: user => {
        if(user) this.loadNotes();
      }
    })
  }

  loadNotes() {
    this.noteService.getNotes().subscribe({
      next: response => {
        this.notes = response
      }
    })
  }

  addNote() {
    this.openPopup(0, "Add Note", true)
  }

  openPopup(id: number, title: string, isAdd: boolean) {
    var _popup = this.dialog.open(PopupComponent, {
      width: '60%',
      enterAnimationDuration: '250ms',
      exitAnimationDuration: '250ms',
      data: {
        isAdd: isAdd,
        title: title,
        id: id
      }
    });
    _popup.afterClosed().subscribe(item => {
      this.loadNotes();
    })
  }

  loginModeToggle(event: boolean) {
    this.loginMode = event;
  }

  editPopupToggle(id: number, title: string, isAdd: boolean)
  {
    this.openPopup(id, title, isAdd);
  }

  onNoteDeletedUpdated() {
    this.loadNotes();
  }

}
