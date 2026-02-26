import { inject, Injectable, signal, computed } from '@angular/core';
import { RubrosService, RubroResumenDto, RubroDto } from '../services/rubros.service';

interface RubrosState {
  rubros: RubroResumenDto[];
  selected: RubroDto | null;
  loading: boolean;
  error: string | null;
  anoFiscal: number;
}

@Injectable({ providedIn: 'root' })
export class RubrosStore {
  private readonly service = inject(RubrosService);

  // Estado reactivo con Signals
  private readonly state = signal<RubrosState>({
    rubros: [],
    selected: null,
    loading: false,
    error: null,
    anoFiscal: new Date().getFullYear()
  });

  // Selectores computados
  readonly rubros = computed(() => this.state().rubros);
  readonly selected = computed(() => this.state().selected);
  readonly loading = computed(() => this.state().loading);
  readonly error = computed(() => this.state().error);
  readonly anoFiscal = computed(() => this.state().anoFiscal);

  readonly totalDisponible = computed(() =>
    this.state().rubros.reduce((sum, r) => sum + r.saldoDisponible, 0)
  );

  readonly rubrosActivos = computed(() =>
    this.state().rubros.filter(r => r.estado === 'Activo')
  );

  readonly promedioEjecucion = computed(() => {
    const rubros = this.rubrosActivos();
    if (rubros.length === 0) return 0;
    return rubros.reduce((sum, r) => sum + r.porcentajeEjecucion, 0) / rubros.length;
  });

  // Acciones
  async cargar(anoFiscal?: number) {
    const ano = anoFiscal ?? this.state().anoFiscal;
    this.state.update(s => ({ ...s, loading: true, error: null, anoFiscal: ano }));

    this.service.listar(ano).subscribe({
      next: rubros => this.state.update(s => ({ ...s, rubros, loading: false })),
      error: err => this.state.update(s => ({ ...s, loading: false, error: err.message }))
    });
  }

  async seleccionar(id: string) {
    this.state.update(s => ({ ...s, loading: true }));
    this.service.obtenerPorId(id).subscribe({
      next: rubro => this.state.update(s => ({ ...s, selected: rubro, loading: false })),
      error: err => this.state.update(s => ({ ...s, loading: false, error: err.message }))
    });
  }

  limpiarSeleccion() {
    this.state.update(s => ({ ...s, selected: null }));
  }
}
