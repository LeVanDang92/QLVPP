import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MenuRolePermission } from './menu-role-permission';

describe('MenuRolePermission', () => {
  let component: MenuRolePermission;
  let fixture: ComponentFixture<MenuRolePermission>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MenuRolePermission],
    }).compileComponents();

    fixture = TestBed.createComponent(MenuRolePermission);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
