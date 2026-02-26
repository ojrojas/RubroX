import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { RubrosStore } from '../../core/store/rubros.store';
import { SaldoIndicatorComponent } from '../../shared/components/domain/saldo-indicator/saldo-indicator.component';

@Component({
  selector: 'rx-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink, SaldoIndicatorComponent],
  template: `
    <div class="dashboard">
      <!-- Page Header -->
      <header class="dashboard__header">
        <div>
          <h1 class="dashboard__title">Dashboard Presupuestal</h1>
          <p class="dashboard__subtitle">Vigencia {{ store.anoFiscal() }}</p>
        </div>
      </header>

      <!-- KPI Cards -->
      <section class="dashboard__kpis" aria-label="Indicadores clave">
        @if (store.loading()) {
          @for (i of [1,2,3,4]; track i) {
            <div class="kpi-card kpi-card--skeleton" aria-busy="true" aria-label="Cargando...">
              <div class="skeleton skeleton--title"></div>
              <div class="skeleton skeleton--value"></div>
            </div>
          }
        } @else {
          <article class="kpi-card">
            <p class="kpi-card__label">Total Apropiado</p>
            <p class="kpi-card__value">{{ formatCop(totalApropiado()) }}</p>
            <p class="kpi-card__description">{{ store.rubrosActivos().length }} rubros activos</p>
          </article>

          <article class="kpi-card kpi-card--ejecutado">
            <p class="kpi-card__label">% Ejecución Promedio</p>
            <p class="kpi-card__value">{{ store.promedioEjecucion() | number:'1.1-1' }}%</p>
            <p class="kpi-card__description">Sobre rubros activos</p>
          </article>

          <article class="kpi-card kpi-card--comprometido">
            <p class="kpi-card__label">Saldo Comprometido</p>
            <p class="kpi-card__value">{{ formatCop(totalComprometido()) }}</p>
            <p class="kpi-card__description">CDP vigentes</p>
          </article>

          <article class="kpi-card kpi-card--disponible">
            <p class="kpi-card__label">Saldo Disponible</p>
            <p class="kpi-card__value">{{ formatCop(store.totalDisponible()) }}</p>
            <p class="kpi-card__description">Para comprometer</p>
          </article>
        }
      </section>

      <!-- Error State -->
      @if (store.error()) {
        <div class="error-banner" role="alert">
          <p>{{ store.error() }}</p>
        </div>
      }

      <!-- Rubros con más ejecución -->
      <section class="dashboard__section">
        <div class="section-header">
          <h2 class="section-header__title">Rubros por Ejecución</h2>
          <a routerLink="/rubros" class="section-header__link">Ver todos →</a>
        </div>

        <div class="rubros-list">
          @for (rubro of topRubros(); track rubro.id) {
            <article class="rubro-card">
              <div class="rubro-card__info">
                <code class="rubro-card__codigo">{{ rubro.codigo }}</code>
                <p class="rubro-card__nombre">{{ rubro.nombre }}</p>
              </div>
              <div class="rubro-card__saldo">
                <rx-saldo-indicator
                  [saldoInicial]="rubroTotal(rubro)"
                  [saldoEjecutado]="0"
                  [saldoComprometido]="0"
                  [saldoDisponible]="rubro.saldoDisponible"
                  [porcentajeEjecucion]="rubro.porcentajeEjecucion"
                />
              </div>
            </article>
          }

          @empty {
            <div class="empty-state">
              <p>No hay rubros disponibles para la vigencia {{ store.anoFiscal() }}</p>
              <a routerLink="/rubros/nuevo" class="btn btn--primary">Crear primer rubro</a>
            </div>
          }
        </div>
      </section>
    </div>
  `,
  styles: [`
    .dashboard { padding: var(--space-6); max-width: var(--content-max-width); margin: 0 auto; }

    .dashboard__header { margin-bottom: var(--space-8); }
    .dashboard__title { font-size: var(--text-2xl); font-weight: var(--font-bold); color: var(--color-text); margin: 0; }
    .dashboard__subtitle { font-size: var(--text-sm); color: var(--color-text-muted); margin: var(--space-1) 0 0; }

    .dashboard__kpis {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(220px, 1fr));
      gap: var(--space-4);
      margin-bottom: var(--space-8);
    }

    .kpi-card {
      background: var(--color-bg);
      border: 1px solid var(--color-border);
      border-radius: var(--radius-lg);
      padding: var(--space-6);
      box-shadow: var(--shadow-sm);
      border-top: 3px solid var(--color-primary-600);
    }
    .kpi-card--ejecutado    { border-top-color: var(--color-ejecutado); }
    .kpi-card--comprometido { border-top-color: var(--color-comprometido); }
    .kpi-card--disponible   { border-top-color: var(--color-disponible); }
    .kpi-card--skeleton     { min-height: 100px; }

    .kpi-card__label { font-size: var(--text-xs); font-weight: var(--font-medium); color: var(--color-text-muted); text-transform: uppercase; letter-spacing: 0.05em; margin: 0 0 var(--space-2); }
    .kpi-card__value { font-size: var(--text-2xl); font-weight: var(--font-bold); color: var(--color-text); margin: 0 0 var(--space-1); }
    .kpi-card__description { font-size: var(--text-xs); color: var(--color-text-subtle); margin: 0; }

    .skeleton { background: linear-gradient(90deg, var(--color-neutral-200) 25%, var(--color-neutral-100) 50%, var(--color-neutral-200) 75%); background-size: 200% 100%; animation: shimmer 1.5s infinite; border-radius: var(--radius-sm); }
    .skeleton--title { height: 12px; width: 60%; margin-bottom: var(--space-3); }
    .skeleton--value { height: 32px; width: 80%; }
    @keyframes shimmer { 0% { background-position: 200% 0; } 100% { background-position: -200% 0; } }

    .error-banner { background: #fef2f2; border: 1px solid #fecaca; border-radius: var(--radius-md); padding: var(--space-4); color: var(--color-error); margin-bottom: var(--space-6); }

    .dashboard__section { background: var(--color-bg); border: 1px solid var(--color-border); border-radius: var(--radius-lg); padding: var(--space-6); box-shadow: var(--shadow-sm); }

    .section-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: var(--space-4); }
    .section-header__title { font-size: var(--text-lg); font-weight: var(--font-semibold); margin: 0; }
    .section-header__link { font-size: var(--text-sm); color: var(--color-primary-600); text-decoration: none; }

    .rubros-list { display: flex; flex-direction: column; gap: var(--space-4); }
    .rubro-card { display: grid; grid-template-columns: 1fr 2fr; gap: var(--space-6); padding: var(--space-4); border: 1px solid var(--color-border); border-radius: var(--radius-md); }
    .rubro-card__codigo { font-family: var(--font-mono); font-size: var(--text-xs); background: var(--color-bg-muted); padding: 2px 6px; border-radius: var(--radius-sm); }
    .rubro-card__nombre { font-size: var(--text-sm); color: var(--color-text); margin: var(--space-1) 0 0; }

    .empty-state { text-align: center; padding: var(--space-12); color: var(--color-text-muted); }
    .btn { display: inline-flex; align-items: center; justify-content: center; padding: var(--space-2) var(--space-4); border-radius: var(--radius-md); font-size: var(--text-sm); font-weight: var(--font-medium); cursor: pointer; border: none; text-decoration: none; }
    .btn--primary { background: var(--color-primary-600); color: white; }

    @media (max-width: 768px) {
      .rubro-card { grid-template-columns: 1fr; }
      .dashboard__kpis { grid-template-columns: repeat(2, 1fr); }
    }
    @media (max-width: 480px) {
      .dashboard__kpis { grid-template-columns: 1fr; }
    }
  `]
})
export class DashboardComponent implements OnInit {
  readonly store = inject(RubrosStore);

  readonly totalApropiado = computed(() =>
    this.store.rubros().reduce((sum, r) => sum + (r.saldoDisponible ?? 0), 0)
  );

  readonly totalComprometido = computed(() => 0);

  readonly topRubros = computed(() =>
    [...this.store.rubros()]
      .sort((a, b) => b.porcentajeEjecucion - a.porcentajeEjecucion)
      .slice(0, 5)
  );

  rubroTotal(rubro: { saldoDisponible: number; porcentajeEjecucion: number }): number {
    const pct = rubro.porcentajeEjecucion;
    if (pct <= 0) return rubro.saldoDisponible;
    return rubro.saldoDisponible / ((100 - pct) / 100);
  }

  ngOnInit(): void {
    this.store.cargar();
  }

  formatCop(value: number): string {
    return new Intl.NumberFormat('es-CO', {
      style: 'currency', currency: 'COP', minimumFractionDigits: 0
    }).format(value);
  }
}
