import { Injectable } from '@angular/core';
import { Note } from '../_modules/note';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class NoteService {
  baseUrl = 'https://localhost:5001/api/'
  notes: Note[] = [];

  constructor(private http: HttpClient) { }

  getNotes() {
    return this.http.get<Note[]>(this.baseUrl + "notes");
  }

  getNote(id: number) {
    return this.http.get<Note>(this.baseUrl + "notes/" + id);
  }

  addNote(model: any) {
    return this.http.post(this.baseUrl + "notes", model);
  }

  updateNote(model: any) {
    return this.http.put(this.baseUrl + "notes", model)
  }

  deleteNote(id: number) {
    return this.http.delete(this.baseUrl + "notes/" + id)
  }
}
