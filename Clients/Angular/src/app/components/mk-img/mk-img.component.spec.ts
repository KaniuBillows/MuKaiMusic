import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MkImgComponent } from './mk-img.component';

describe('MkImgComponent', () => {
  let component: MkImgComponent;
  let fixture: ComponentFixture<MkImgComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MkImgComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MkImgComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
