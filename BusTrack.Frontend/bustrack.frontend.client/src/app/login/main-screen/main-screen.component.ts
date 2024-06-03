import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { preventBackNavigationRule } from'../rules/preventBackNavigationRules/preventBackNavigationRule';
import { preventForwardNavigationRule } from '../rules/preventForwardNavigationRules/preventForwardNavigationRule';
@Component({
  selector: 'app-main-screen',
  templateUrl: './main-screen.component.html',
  styleUrls: ['./main-screen.component.css']
})

export class MainScreenComponent implements OnInit {
  inactivityTimer: any; 
  INACTIVITY_TIMEOUT_MS = 1200000; 

  constructor(
    private router: Router,
  ) { }

  entrarNoSistema() {
    this.router.navigate(['/enter-the-system']);
  }

  criarConta() {
    this.router.navigate(['/create-an-account']);
  }

  ngOnInit(): void {
    preventBackNavigationRule();
    preventForwardNavigationRule();
  }
}
