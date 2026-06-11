import { Component, inject, signal, OnInit, computed, effect } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { User } from '../../shared/models/User';
import { SetupPageLayout } from '../../shared/layout/setup-page-layout/setup-page-layout';
import { ActivatedRoute } from '@angular/router';
import { UserService } from '../../services/user.service';
import { RoleService } from '../../services/role.service';
import { Role } from '../../shared/models/Role';
import { CodedataService } from '../../../../core/services/codedata.service';
import { CodeDataDto } from '../../../../core/models/codeDataDto';
import { FormErrorDirective } from '../../../../shared/Directive/form-error.directive';
import { FormSignalService } from '../../../../shared/services/FormSignalService';

@Component({
  selector: 'app-register',
  imports: [SetupPageLayout, ReactiveFormsModule, FormErrorDirective],
  templateUrl: './register.html',
  styleUrl: './register.scss',
})
export class Register implements OnInit {

  private userService = inject(UserService);
  private roleService = inject(RoleService);
  private codeDataService = inject(CodedataService)
  private formSignalService = inject(FormSignalService);

  private route = inject(ActivatedRoute);
  private fb = inject(FormBuilder);

  breadcrumb = signal(this.route.snapshot.data['breadcrumb'] ?? '');
  title = signal(this.route.snapshot.data['title'] ?? '');
  sectionTitle = signal(this.route.snapshot.data['sectionTitle'] ?? '');

  isSaving = signal(false);
  selectedUser = signal<User | null>(null);

  users = signal<User[]>([]);
  roles = signal<Role[]>([]);
  departments = signal<CodeDataDto[]>([]);


  constructor() {
       effect(() => {
           const user = this.selectedUser();
           const userIdControl = this.form.get('userId');

           if (user) {
             userIdControl?.disable();
           } else {
             userIdControl?.enable();
           }
       });
  }

  ngOnInit() {
    this.loadUsers();
    this.loadRoles();
    this.loadCodeData();
  }

  private loadUsers(): void {
    this.userService.getUsers().subscribe({
      next: (users) => this.users.set(users),
      error: (error) => {
        console.error('Failed to load users:', error);
        alert('Failed to load users. Please try again later.');
      }
    });
  }

  private loadRoles(): void {
    this.roleService.getRoles().subscribe({
      next: (roles) => this.roles.set(roles),
      error: (error) => {
        console.error('Failed to load roles:', error);
        alert('Failed to load roles. Please try again later.');
      }
    });
  }

  private loadCodeData(): void {
    this.codeDataService.getCodeData('DEPT').subscribe({
      next: (codeDatas) => {

         this.departments.set(codeDatas);;
      },
      error: (error) => {
        console.error('Failed to load code data:', error);
        alert('Failed to load code data. Please try again later.');
      }
    });
  }

  form = this.fb.group({
    userId: ['', Validators.required],
    userName: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    department: [''],
    isActive: [true, Validators.required],
    role: ['', Validators.required],
    passwordShow: ['', [Validators.required, Validators.minLength(6)]],
    isShowPassword: [false],
  });

  selectUser(user: User): void {
    this.selectedUser.set(user);
    this.form.patchValue(user);
  }

  save(): void {
    if (this.form.invalid) {
     // alert('Please fill in all required fields correctly.');
      this.form.markAllAsTouched();
      this.formSignalService.triggerSave();
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
      userId: this.selectedUser()?.userId || this.form.value.userId || '',
      fullName:  '',
      passwordShow: this.form.value.passwordShow || '',
      createdAt: this.selectedUser()?.createdAt || new Date(),
      createdBy: this.selectedUser()?.createdBy || 'Admin',
      modifiedAt: new Date(),
      modifiedBy: 'Admin',
    };

    const existingUsers = this.users();
    const userIndex = existingUsers.findIndex((u) => u.userId === userToSave.userId);

    if (userIndex > -1) {
      // Update existing user
      this.UpdateUser(userToSave);
    } else {
      // Add new user
      this.AddNewUser(userToSave);
    }
  }

  onCreate(): void {
    this.save();
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
    this.DeleteUser(this.selectedUser()!.userId);
  }

  onReset(): void {
     this.form.reset({ isActive: true });
     this.selectedUser.set(null);
  }

  private AddNewUser(user: User): void {

    this.userService.registerUser(user).subscribe({
      next: (newUserId) => {
        const updatedUsers = [...this.users(), user];
        this.users.set(updatedUsers);
        this.selectedUser.set(null);
        this.form.reset({ isActive: true });
        alert('User registered successfully!');
      },
      error: (error) => {
        console.error('Failed to register user:', error);
        alert('Failed to register user. Please try again later.');
      }
    });

  }

  private UpdateUser(user: User): void {
    this.userService.updateUser(user).subscribe({
      next: (updatedUser) => {
        const existingUsers = this.users();
        const userIndex = existingUsers.findIndex((u) => u.userId === updatedUser.userId);
        if (userIndex > -1) {
          existingUsers[userIndex] = updatedUser;
          this.users.set([...existingUsers]);
          this.selectedUser.set(null);
          this.form.reset({ isActive: true });
          alert('User updated successfully!');
        }
      },
      error: (error) => {
        console.error('Failed to update user:', error);
        alert('Failed to update user. Please try again later.');
      }
    });
  }

  private DeleteUser(userId: string): void {
    this.userService.deleteUser(userId).subscribe({
      next: () => {
        const existingUsers = this.users();
        const updatedUsers = existingUsers.filter((u) => u.userId !== userId);
        this.users.set(updatedUsers);
        this.selectedUser.set(null);
        this.form.reset({ isActive: true });
        alert('User deleted successfully!');
      },
      error: (error) => {
        console.error('Failed to delete user:', error);
        alert('Failed to delete user. Please try again later.');
      }
    });
  }
}
