import { Component, computed, inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { map } from 'rxjs';

@Component({
  selector: 'app-workspace-page',
  standalone: true,
  templateUrl: './workspace-page.component.html',
  styleUrl: './workspace-page.component.scss',
})
export class WorkspacePageComponent {
  private readonly route = inject(ActivatedRoute);

  private readonly slug = toSignal(this.route.paramMap.pipe(map((params) => params.get('slug') ?? 'page')), {
    initialValue: 'page',
  });

  readonly title = computed(() =>
    this.slug()
      .split('-')
      .filter(Boolean)
      .map((word) => word.charAt(0).toUpperCase() + word.slice(1))
      .join(' '),
  );
}
