import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SetupToolbar } from './setup-toolbar';

describe('SetupToolbar', () => {
  let component: SetupToolbar;
  let fixture: ComponentFixture<SetupToolbar>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SetupToolbar],
    }).compileComponents();

    fixture = TestBed.createComponent(SetupToolbar);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
