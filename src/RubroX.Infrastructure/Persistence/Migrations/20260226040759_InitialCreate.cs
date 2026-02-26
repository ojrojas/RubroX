using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RubroX.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "presupuesto");

            migrationBuilder.CreateTable(
                name: "flujos_aprobacion",
                schema: "presupuesto",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    rubro_id = table.Column<Guid>(type: "uuid", nullable: true),
                    movimiento_id = table.Column<Guid>(type: "uuid", nullable: true),
                    tipo_flujo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    estado = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    paso_actual = table.Column<int>(type: "integer", nullable: false),
                    iniciador_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    payload = table.Column<string>(type: "jsonb", nullable: false),
                    fecha_inicio = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    fecha_fin = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_flujos_aprobacion", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "movimientos",
                schema: "presupuesto",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    rubro_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tipo = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    monto = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    concepto = table.Column<string>(type: "text", nullable: false),
                    numeracion = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    usuario_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    movimiento_padre_id = table.Column<Guid>(type: "uuid", nullable: true),
                    fecha_registro = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    fecha_vencimiento = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    estado = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    observacion = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_movimientos", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "rubros",
                schema: "presupuesto",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    codigo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    nombre = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    descripcion = table.Column<string>(type: "text", nullable: false),
                    ano_fiscal = table.Column<int>(type: "integer", nullable: false),
                    tipo = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    fuente = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    saldo_inicial = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    saldo_comprometido = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    saldo_ejecutado = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    estado = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    padre_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rubros", x => x.id);
                    table.ForeignKey(
                        name: "FK_rubros_rubros_padre_id",
                        column: x => x.padre_id,
                        principalSchema: "presupuesto",
                        principalTable: "rubros",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "pasos_aprobacion",
                schema: "presupuesto",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    flujo_id = table.Column<Guid>(type: "uuid", nullable: false),
                    orden = table.Column<int>(type: "integer", nullable: false),
                    rol_requerido = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    aprobador_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    estado = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    comentario = table.Column<string>(type: "text", nullable: true),
                    fecha_accion = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pasos_aprobacion", x => x.id);
                    table.ForeignKey(
                        name: "FK_pasos_aprobacion_flujos_aprobacion_flujo_id",
                        column: x => x.flujo_id,
                        principalSchema: "presupuesto",
                        principalTable: "flujos_aprobacion",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "idx_flujos_estado",
                schema: "presupuesto",
                table: "flujos_aprobacion",
                column: "estado");

            migrationBuilder.CreateIndex(
                name: "idx_flujos_rubro",
                schema: "presupuesto",
                table: "flujos_aprobacion",
                column: "rubro_id");

            migrationBuilder.CreateIndex(
                name: "idx_movimientos_estado",
                schema: "presupuesto",
                table: "movimientos",
                column: "estado");

            migrationBuilder.CreateIndex(
                name: "idx_movimientos_rubro",
                schema: "presupuesto",
                table: "movimientos",
                column: "rubro_id");

            migrationBuilder.CreateIndex(
                name: "idx_movimientos_tipo",
                schema: "presupuesto",
                table: "movimientos",
                column: "tipo");

            migrationBuilder.CreateIndex(
                name: "uq_movimiento_numeracion",
                schema: "presupuesto",
                table: "movimientos",
                column: "numeracion",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uq_flujo_orden",
                schema: "presupuesto",
                table: "pasos_aprobacion",
                columns: new[] { "flujo_id", "orden" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_rubros_ano_fiscal",
                schema: "presupuesto",
                table: "rubros",
                column: "ano_fiscal");

            migrationBuilder.CreateIndex(
                name: "idx_rubros_estado",
                schema: "presupuesto",
                table: "rubros",
                column: "estado");

            migrationBuilder.CreateIndex(
                name: "IX_rubros_padre_id",
                schema: "presupuesto",
                table: "rubros",
                column: "padre_id");

            migrationBuilder.CreateIndex(
                name: "uq_rubro_codigo_ano",
                schema: "presupuesto",
                table: "rubros",
                columns: new[] { "codigo", "ano_fiscal" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "movimientos",
                schema: "presupuesto");

            migrationBuilder.DropTable(
                name: "pasos_aprobacion",
                schema: "presupuesto");

            migrationBuilder.DropTable(
                name: "rubros",
                schema: "presupuesto");

            migrationBuilder.DropTable(
                name: "flujos_aprobacion",
                schema: "presupuesto");
        }
    }
}
