import { Component, input, output } from '@angular/core';
import { SetupToolbar } from '../../components/setup-toolbar/setup-toolbar';

@Component({
  selector: 'app-setup-page-layout',
  imports: [SetupToolbar],
  templateUrl: './setup-page-layout.html',
  styleUrl: './setup-page-layout.scss',
})
export class SetupPageLayout {

  breadcrumb = input<string>('');
  title = input<string>('');
  sectionTitle = input<string>('');

  createDisabled = input(false);
  updateDisabled = input(true);
  deleteDisabled = input(true);
  resetDisabled = input(false);

  create = output<void>();
  update = output<void>();
  delete = output<void>();
  reset = output<void>();
}
