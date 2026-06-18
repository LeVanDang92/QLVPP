import { Component, inject, OnInit, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { LanguageService } from './core/services/language.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class AppComponent implements OnInit {
  protected readonly title = signal('OSM.UI');

  private readonly languageService = inject(LanguageService)

  ngOnInit(): void {
    this.languageService.initLanguage();
  }
}
