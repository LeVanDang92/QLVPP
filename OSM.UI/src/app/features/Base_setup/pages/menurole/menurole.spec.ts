import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Menurole } from './menurole';

describe('Menurole', () => {
  let component: Menurole;
  let fixture: ComponentFixture<Menurole>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Menurole],
    }).compileComponents();

    fixture = TestBed.createComponent(Menurole);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
