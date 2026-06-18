import { Component, inject, OnInit, signal } from '@angular/core';
import { SetupPageLayout } from '../../shared/layout/setup-page-layout/setup-page-layout';
import { ActivatedRoute } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RoleService } from '../../services/role.service';
import { Role } from '../../shared/models/Role';
import { FormErrorDirective } from '../../../../shared/Directive/form-error.directive';
import { FormSignalService } from '../../../../shared/services/FormSignalService';
import { ServerValidationErrorService } from '../../../../shared/services/server-validation-error.service';

@Component({
  selector: 'app-role',
  imports: [SetupPageLayout, ReactiveFormsModule, FormErrorDirective],
  templateUrl: './role.html',
  styleUrl: './role.scss',
})
export class RoleComponent implements OnInit {
  private roleService = inject(RoleService);

  private route = inject(ActivatedRoute);
  private fb = inject(FormBuilder);
  private formSignalService = inject(FormSignalService);
  private serverValidationErrorService = inject(ServerValidationErrorService);

  breadcrumb = signal(this.route.snapshot.data['breadcrumb'] ?? '');
  title = signal(this.route.snapshot.data['title'] ?? '');
  sectionTitle = signal(this.route.snapshot.data['sectionTitle'] ?? '');

  selectedRole = signal<Role | null>(null);
  roles = signal<Role[]>([]);

  ngOnInit() {
    this.loadRoles();
  }

  form = this.fb.group({
    name: ['', Validators.required],
    description: ['', Validators.maxLength(150)],
  });

  loadRoles() {
    this.roleService.getRoles().subscribe({
      next: (roles) => this.roles.set(roles),
      error: (error) => {
        console.error('Failed to load roles:', error);
        alert('Failed to load roles. Please try again later.');
      },
    });
  }

  selectRole(role: Role) {
    this.selectedRole.set(role);
    this.form.patchValue({
      name: role.name,
      description: role.description,
    });
  }

  onCreate() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      this.formSignalService.triggerSave();
      return;
    }

    const newRole: Role = {
      id: '',
      name: this.form.value.name!,
      description: this.form.value.description!,
    };

    this.roleService.createRole(newRole).subscribe({
      next: (createdRole) => {
        alert('Role created successfully!');
        this.roles.update((currentRoles) => [...currentRoles, createdRole]);
        this.form.reset();
        this.selectedRole.set(null);
      },
      error: (error) => {
        this.serverValidationErrorService.applyErrors(this.form, error);
        alert('Failed to create role. Please try again later.');
      },
    });
  }

  onUpdate() {
    if (this.form.invalid || !this.selectedRole()) {
      this.form.markAllAsTouched();
      this.formSignalService.triggerSave();
      return;
    }

    const updatedRole: Role = {
      ...this.selectedRole()!,
      name: this.form.value.name!,
      description: this.form.value.description!,
    };

    if (!confirm(`Are you sure you want to update role "${updatedRole.name}"?`)) {
      return;
    }

    this.roleService.updateRole(updatedRole).subscribe({
      next: (role) => {
        alert('Role updated successfully!');
        this.roles.update((currentRoles) => currentRoles.map((r) => (r.id === role.id ? role : r)));
        this.form.reset();
        this.selectedRole.set(null);
      },
      error: (error) => {
        this.serverValidationErrorService.applyErrors(this.form, error);
        alert('Failed to update role. Please try again later.');
      },
    });
  }

  onDelete() {
    if (!this.selectedRole()) {
      alert('Please select a role to delete.');
      return;
    }

    if (!confirm('Are you sure you want to delete this role?')) {
      return;
    }

    this.roleService.deleteRole(this.selectedRole()!.id).subscribe({
      next: () => {
        alert('Role deleted successfully!');
        this.roles.update((currentRoles) =>
          currentRoles.filter((r) => r.id !== this.selectedRole()!.id),
        );
        this.form.reset();
        this.selectedRole.set(null);
      },
      error: (error) => {
        this.serverValidationErrorService.applyErrors(this.form, error);
        alert('Failed to delete role. Please try again later.');
      },
    });
  }

  onReset() {
    this.selectedRole.set(null);
    this.form.reset();
  }
}
