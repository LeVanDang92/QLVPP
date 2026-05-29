import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { User } from '../../shared/models/User';
import { SetupPageLayout } from '../../shared/layout/setup-page-layout/setup-page-layout';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-register',
  imports: [SetupPageLayout, ReactiveFormsModule],
  templateUrl: './register.html',
  styleUrl: './register.scss',
})
export class Register {

  private route = inject(ActivatedRoute);
  private fb = inject(FormBuilder);

  breadcrumb = signal(this.route.snapshot.data['breadcrumb'] ?? '');
  title = signal(this.route.snapshot.data['title'] ?? '');
  sectionTitle = signal(this.route.snapshot.data['sectionTitle'] ?? '');

  isSaving = signal(false);
  selectedUser = signal<User | null>(null);

   users = signal<User[]>([
    {
      userId: '2090905', userName: '송영주', email: 'danglevan@gmail.com', department: 'IT', isActive: true, role: 'SITE_A', createdAt: null, createdBy: null, modifiedAt: null, modifiedBy: null,
      fullName: null,
      passwordShow: '12345'
    },
    {
      userId: '2101002', userName: '고재관', email: 'john.doe@gmail.com', department: 'HR', isActive: true, role: 'SITE_B', createdAt: null, createdBy: null, modifiedAt: null, modifiedBy: null,
      fullName: null,
      passwordShow: '12345'
    },
    {
      userId: '2121030', userName: 'Jeremy', email: 'jane.smith@gmail.com', department: 'IT', isActive: true, role: 'SITE_A', createdAt: null, createdBy: null, modifiedAt: null, modifiedBy: null,
      fullName: null,
      passwordShow: '12345'
    }
  ]);

  form = this.fb.group({
    userId : ['',Validators.required],
    userName: ['',Validators.required],
    fullName: ['',Validators.required],
    email: ['',[Validators.required, Validators.email]],
    department: ['',Validators.required],
    isActive: [true,Validators.required],
    role: ['',Validators.required],
    passwordShow: ['',Validators.required,Validators.minLength(6)],
    isShowPassword : [false]
  });

  selectUser(user: User): void {
    this.selectedUser.set(user);
    this.form.patchValue(user);
  }

  save(): void {
    if (this.form.invalid) {
      alert('Please fill in all required fields correctly.');
      return;
    }

    const userToSave: User = {
      ...this.selectedUser(),
      ...this.form.value,
      userName: this.form.value.userName || '',
      email: this.form.value.email || '',
      department: this.form.value.department || '',
      isActive: this.form.value.isActive ?? true,
      role: this.form.value.role || '',
      userId: this.selectedUser()?.userId || this.form.value.userId || this.generateId(),
      fullName: this.form.value.fullName || '',
      passwordShow: '1234567890',
      createdAt: this.selectedUser()?.createdAt || new Date(),
      createdBy: this.selectedUser()?.createdBy || 'Admin',
      modifiedAt: new Date(),
      modifiedBy: 'Admin'
    };

    const existingUsers = this.users();
    const userIndex = existingUsers.findIndex(u => u.userId === userToSave.userId);

    if (userIndex > -1) {
      // Update existing user
      existingUsers[userIndex] = userToSave;
    } else {
      // Add new user
      existingUsers.push(userToSave);
    }

    this.users.set([...existingUsers]);
    this.selectedUser.set(null);
    this.form.reset({ isActive: true });
  }

  private generateId(): string {
    return Math.random().toString(36).substr(2, 9);
  }

  onCreate(): void {
    this.selectedUser.set(null);
    this.form.reset({ isActive: true });
  }

  onUpdate(): void {
    if (!this.selectedUser()) {
      alert('Please select a user to update.');
      return;
    }
    this.save();
  }

  onDelete(): void {
    if (!this.selectedUser()) {
      alert('Please select a user to delete.');
      return;
    }
    const existingUsers = this.users();
    const updatedUsers = existingUsers.filter(u => u.userId !== this.selectedUser()?.userId);
    this.users.set(updatedUsers);
    this.selectedUser.set(null);
    this.form.reset({ isActive: true });
  }

  onReset(): void {
    if (this.selectedUser()) {
      this.form.patchValue(this.selectedUser()!);
    } else {
      this.form.reset({ isActive: true });
    }
  }

}
