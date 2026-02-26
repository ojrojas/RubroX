import { inject, Injectable, signal, computed } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface RubroResumenDto {
  id: string;
  codigo: string;
  nombre: string;
  estado: string;
  saldoDisponible: number;
  porcentajeEjecucion: number;
  totalHijos: number;
}

export interface RubroDto extends RubroResumenDto {
  descripcion: string;
  anoFiscal: number;
  tipo: string;
  fuente: string;
  saldoInicial: number;
  saldoComprometido: number;
  saldoEjecutado: number;
  padreId?: string;
  creadoEn: string;
  actualizadoEn: string;
  creadoPor?: string;
  hijos?: RubroDto[];
}

export interface CrearRubroRequest {
  codigo: string;
  nombre: string;
  descripcion?: string;
  anoFiscal: number;
  tipo: string;
  fuente: string;
  padreId?: string;
}

@Injectable({ providedIn: 'root' })
export class RubrosService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/api/v1/rubros`;

  listar(anoFiscal: number, estado?: string): Observable<RubroResumenDto[]> {
    let params = new HttpParams().set('anoFiscal', anoFiscal);
    if (estado) params = params.set('estado', estado);
    return this.http.get<RubroResumenDto[]>(this.baseUrl, { params });
  }

  obtenerPorId(id: string): Observable<RubroDto> {
    return this.http.get<RubroDto>(`${this.baseUrl}/${id}`);
  }

  crear(request: CrearRubroRequest): Observable<{ id: string }> {
    return this.http.post<{ id: string }>(this.baseUrl, request);
  }

  asignarPresupuesto(id: string, monto: number): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}/presupuesto`, { monto });
  }
}
