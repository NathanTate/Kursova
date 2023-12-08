import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { Note } from '../_modules/note';
import { NoteService } from '../_services/note.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-note-card',
  templateUrl: './note-card.component.html',
  styleUrls: ['./note-card.component.css']
})
export class NoteCardComponent implements OnInit{
  @Output() openPopup = new EventEmitter();
  @Output() onNoteDelete = new EventEmitter<void>();
  @Input() note: Note | undefined;

  constructor(private noteService: NoteService, private toastr: ToastrService) {}
  
  ngOnInit(): void {
  }

  editNote(id: number) {
    this.openPopup.emit([id, "Edit note", false])
  }

  deleteNote(id: number) {
    this.noteService.deleteNote(id).subscribe({
      next: () => {
      this.onNoteDelete.emit(),
      this.toastr.success("Note deleted successfully")
      }
    })
  }
}
