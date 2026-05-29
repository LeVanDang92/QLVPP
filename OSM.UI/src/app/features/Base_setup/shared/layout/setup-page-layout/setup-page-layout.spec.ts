import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SetupPageLayout } from './setup-page-layout';

describe('SetupPageLayout', () => {
  let component: SetupPageLayout;
  let fixture: ComponentFixture<SetupPageLayout>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SetupPageLayout],
    }).compileComponents();

    fixture = TestBed.createComponent(SetupPageLayout);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
