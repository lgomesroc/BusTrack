import { TestBed } from '@angular/core/testing';

import { OperationalReportExportService } from './operational-report-export.service';

describe('OperationalReportExportService', () => {
  let service: OperationalReportExportService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(OperationalReportExportService);
  });

  it('deve gerar csv com cabeçalho e linhas', () => {
    const csv = service.buildCsv(
      ['Ônibus', 'Viagens'],
      [['001', 4], ['002', 7]]
    );

    expect(csv).toContain('Ônibus,Viagens');
    expect(csv).toContain('001,4');
    expect(csv).toContain('002,7');
  });

  it('deve escapar vírgulas, aspas e quebras de linha', () => {
    const csv = service.buildCsv(
      ['Motorista', 'Observação'],
      [['João, Silva', 'Linha "Centro"\nTurno manhã']]
    );

    expect(csv).toContain('"João, Silva","Linha ""Centro""\nTurno manhã"');
  });
});
