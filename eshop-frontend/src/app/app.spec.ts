import { TestBed } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';

import { App } from './app';

/**
 * Test base del componente root dell’applicazione.
 * Verifica che l’App venga creata correttamente
 * e che la struttura principale (shell) sia presente.
 */
describe('AppComponent', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        App,          // standalone component
        RouterTestingModule,   // necessario per router-outlet
      ],
    }).compileComponents();
  });

  it('should create the app', () => {
    const fixture = TestBed.createComponent(App);
    const app = fixture.componentInstance;

    expect(app).toBeTruthy();
  });

  it('should render the application shell', () => {
    const fixture = TestBed.createComponent(App);
    fixture.detectChanges();

    const compiled = fixture.nativeElement as HTMLElement;

    // Verifica presenza layout principale
    expect(compiled.querySelector('.app-shell')).toBeTruthy();
    expect(compiled.querySelector('header.app-header')).toBeTruthy();
    expect(compiled.querySelector('main.app-main')).toBeTruthy();
  });
});
