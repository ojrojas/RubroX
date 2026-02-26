import { Component, input, computed } from '@angular/core';
import { CommonModule } from '@angular/common';

/**
 * SaldoIndicator — Componente exclusivo de RubroX.
 * Muestra la barra de ejecución presupuestal tripartita:
 * Ejecutado (azul) | Comprometido (ámbar) | Disponible (verde)
 */
@Component({
  selector: 'rx-saldo-indicator',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="saldo-indicator" role="img" [attr.aria-label]="ariaLabel()">
      <!-- Cifras -->
      <div class="saldo-indicator__header">
        <span class="saldo-indicator__total">{{ formatCop(saldoInicial()) }}</span>
        <span class="saldo-indicator__pct" [class]="pctClass()">{{ porcentajeEjecucion() | number:'1.1-1' }}%</span>
      </div>

      <!-- Barra tripartita -->
      <div class="saldo-indicator__bar" aria-hidden="true">
        <div
          class="saldo-indicator__segment saldo-indicator__segment--ejecutado"
          [style.width.%]="pctEjecutado()"
          [title]="'Ejecutado: ' + formatCop(saldoEjecutado())"
        ></div>
        <div
          class="saldo-indicator__segment saldo-indicator__segment--comprometido"
          [style.width.%]="pctComprometido()"
          [title]="'Comprometido: ' + formatCop(saldoComprometido())"
        ></div>
      </div>

      <!-- Leyenda -->
      <div class="saldo-indicator__legend">
        <span class="legend-item legend-item--ejecutado">Ejec. {{ formatCop(saldoEjecutado()) }}</span>
        <span class="legend-item legend-item--comprometido">Comp. {{ formatCop(saldoComprometido()) }}</span>
        <span class="legend-item legend-item--disponible">Disp. {{ formatCop(saldoDisponible()) }}</span>
      </div>
    </div>
  `,
  styles: [`
    .saldo-indicator { width: 100%; }

    .saldo-indicator__header {
      display: flex;
      justify-content: space-between;
      align-items: baseline;
      margin-bottom: var(--space-2);
    }

    .saldo-indicator__total {
      font-size: var(--text-sm);
      font-weight: var(--font-semibold);
      color: var(--color-text);
    }

    .saldo-indicator__pct {
      font-size: var(--text-sm);
      font-weight: var(--font-medium);
    }
    .pct-normal { color: var(--color-disponible); }
    .pct-warning { color: var(--color-comprometido); }
    .pct-danger  { color: var(--color-error); }

    .saldo-indicator__bar {
      height: 8px;
      background-color: var(--color-neutral-200);
      border-radius: var(--radius-full);
      overflow: hidden;
      display: flex;
    }

    .saldo-indicator__segment {
      height: 100%;
      transition: width var(--transition-normal);
      min-width: 0;
    }
    .saldo-indicator__segment--ejecutado    { background-color: var(--color-ejecutado); }
    .saldo-indicator__segment--comprometido { background-color: var(--color-comprometido); }

    .saldo-indicator__legend {
      display: flex;
      gap: var(--space-4);
      margin-top: var(--space-2);
      flex-wrap: wrap;
    }

    .legend-item {
      display: flex;
      align-items: center;
      gap: var(--space-1);
      font-size: var(--text-xs);
      color: var(--color-text-muted);
    }
    .legend-item::before {
      content: '';
      display: inline-block;
      width: 8px;
      height: 8px;
      border-radius: 2px;
    }
    .legend-item--ejecutado::before    { background-color: var(--color-ejecutado); }
    .legend-item--comprometido::before { background-color: var(--color-comprometido); }
    .legend-item--disponible::before   { background-color: var(--color-disponible); }
  `]
})
export class SaldoIndicatorComponent {
  readonly saldoInicial = input.required<number>();
  readonly saldoEjecutado = input.required<number>();
  readonly saldoComprometido = input.required<number>();
  readonly saldoDisponible = input.required<number>();
  readonly porcentajeEjecucion = input.required<number>();

  readonly pctEjecutado = computed(() =>
    this.saldoInicial() > 0 ? (this.saldoEjecutado() / this.saldoInicial()) * 100 : 0
  );

  readonly pctComprometido = computed(() =>
    this.saldoInicial() > 0 ? (this.saldoComprometido() / this.saldoInicial()) * 100 : 0
  );

  readonly pctClass = computed(() => {
    const pct = this.porcentajeEjecucion();
    if (pct >= 90) return 'saldo-indicator__pct pct-danger';
    if (pct >= 70) return 'saldo-indicator__pct pct-warning';
    return 'saldo-indicator__pct pct-normal';
  });

  readonly ariaLabel = computed(() =>
    `Ejecución presupuestal: ${this.porcentajeEjecucion().toFixed(1)}% de ${this.formatCop(this.saldoInicial())}`
  );

  formatCop(value: number): string {
    return new Intl.NumberFormat('es-CO', {
      style: 'currency', currency: 'COP', minimumFractionDigits: 0, maximumFractionDigits: 0
    }).format(value);
  }
}
