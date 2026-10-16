import {
  Component,
  OnInit,
  AfterViewInit,
  OnDestroy,
  ElementRef,
  ViewChild,
  ChangeDetectorRef
} from '@angular/core';

import { CommonModule } from '@angular/common';
import { Chart } from 'chart.js/auto';

import { Officer } from '../officer';
import { Sidebar } from '../sidebar/sidebar';

@Component({
  selector: 'app-officer-dashboard',
  standalone: true,
  imports: [CommonModule, Sidebar],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css'
})
export class Dashboard implements OnInit, AfterViewInit, OnDestroy {

  dashboardData: any = null;
  errorMessage = '';
  loading = true;

  @ViewChild('portfolioChart')
  portfolioCanvas?: ElementRef<HTMLCanvasElement>;

  @ViewChild('collectionChart')
  collectionCanvas?: ElementRef<HTMLCanvasElement>;

  private portfolioChart?: Chart;
  private collectionChart?: Chart;
  private viewReady = false;

  constructor(
    private officerService: Officer,
    private cdr: ChangeDetectorRef
  ) {}

  // Automatically load data when the page opens
  ngOnInit(): void {
    this.loadDashboard();
  }

  ngAfterViewInit(): void {
    this.viewReady = true;

    if (this.dashboardData) {
      this.renderCharts();
    }
  }

  // Fetch dashboard data from the API
  loadDashboard(): void {
    this.loading = true;
    this.errorMessage = '';

    this.officerService.getOfficerDashboard().subscribe({
      next: (response: any) => {
        this.dashboardData = response;
        this.loading = false;

        console.log('Dashboard data:', response);

        // Update the HTML before accessing canvas elements
        this.cdr.detectChanges();

        if (this.viewReady) {
          this.renderCharts();
        }
      },

      error: (error: any) => {
        console.error('Dashboard API error:', error);

        this.errorMessage = 'Unable to load dashboard data.';
        this.loading = false;

        this.cdr.detectChanges();
      }
    });
  }

  // Application funnel data
  get funnelItems(): any[] {
    return this.dashboardData?.applicationFunnel ?? [];
  }

  get funnelMaximum(): number {
    const counts = this.funnelItems.map(
      (item: any) => Number(item.count) || 0
    );

    return Math.max(
      Number(
        this.dashboardData?.applicationSummary?.totalApplications
      ) || 0,
      ...counts,
      1
    );
  }

  // Calculate funnel stage widths
  getFunnelWidth(index: number): number {
    const total = this.funnelItems.length;

    if (total <= 1) {
      return 100;
    }

    return 100 - index * (70 / total);
  }

  // Render charts after the API data and view are ready
  renderCharts(): void {
    if (!this.dashboardData || !this.viewReady) {
      return;
    }

    this.createPortfolioChart();
    this.createCollectionChart();
  }

  // Loan Portfolio Doughnut Chart
  createPortfolioChart(): void {
    const canvas = this.portfolioCanvas?.nativeElement;

    if (!canvas) {
      return;
    }

    this.portfolioChart?.destroy();

    const portfolio = this.dashboardData?.loanPortfolio ?? [];

    this.portfolioChart = new Chart(canvas, {
      type: 'doughnut',

      data: {
        labels: portfolio.map(
          (item: any) => item.loanType
        ),

        datasets: [
          {
            label: 'Total Loan Amount',

            data: portfolio.map(
              (item: any) =>
                Number(item.totalLoanAmount) || 0
            ),

            backgroundColor: [
              '#2471d8',
              '#20a779',
              '#f4b942',
              '#8e63ce',
              '#ef6868',
              '#16a6bd'
            ],

            borderColor: '#ffffff',
            borderWidth: 3,
            hoverOffset: 8
          }
        ]
      },

      options: {
        responsive: true,
        maintainAspectRatio: false,
        cutout: '60%',

        plugins: {
          legend: {
            position: 'bottom',

            labels: {
              padding: 18,
              usePointStyle: true
            }
          },

          tooltip: {
            callbacks: {
              label: (context: any) => {
                const value = Number(context.raw) || 0;

                return `${context.label}: ₹${value.toLocaleString('en-IN')}`;
              }
            }
          }
        }
      }
    });
  }

  // Collection Efficiency Bar Chart
  createCollectionChart(): void {
    const canvas = this.collectionCanvas?.nativeElement;

    if (!canvas) {
      return;
    }

    this.collectionChart?.destroy();

    const collection =
      this.dashboardData?.collectionEfficiency;

    const expected =
      Number(collection?.expectedAmount) || 0;

    const collected =
      Number(collection?.collectedAmount) || 0;

    this.collectionChart = new Chart(canvas, {
      type: 'bar',

      data: {
        labels: ['Collection'],

        datasets: [
          {
            label: 'Expected Amount',
            data: [expected],
            backgroundColor: '#2471d8',
            borderRadius: 5,
            maxBarThickness: 55
          },

          {
            label: 'Collected Amount',
            data: [collected],
            backgroundColor: '#20a779',
            borderRadius: 5,
            maxBarThickness: 55
          }
        ]
      },

      options: {
        responsive: true,
        maintainAspectRatio: false,

        scales: {
          y: {
            beginAtZero: true,

            ticks: {
              callback: (value: string | number) =>
                Number(value).toLocaleString('en-IN')
            }
          }
        },

        plugins: {
          legend: {
            position: 'bottom',

            labels: {
              padding: 18,
              usePointStyle: true
            }
          },

          tooltip: {
            callbacks: {
              label: (context: any) => {
                const value = Number(context.raw) || 0;

                return `${context.dataset.label}: ₹${value.toLocaleString('en-IN')}`;
              }
            }
          }
        }
      }
    });
  }

  // Clean up chart instances when leaving the page
  ngOnDestroy(): void {
    this.portfolioChart?.destroy();
    this.collectionChart?.destroy();
  }
}