import { Component, input, output } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';

@Component({
  selector: 'app-setup-toolbar',
  imports: [TranslatePipe],
  templateUrl: './setup-toolbar.html',
  styleUrl: './setup-toolbar.scss',
})
export class SetupToolbar {

  createDisabled = input(false);
  updateDisabled = input(true);
  deleteDisabled = input(true);
  resetDisabled = input(false);

  create = output<void>();
  update = output<void>();
  delete = output<void>();
  reset = output<void>();

}
