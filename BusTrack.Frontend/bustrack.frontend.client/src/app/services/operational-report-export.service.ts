import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class OperationalReportExportService {
  buildCsv(headers: string[], rows: Array<Array<string | number>>): string {
    const lines = [headers, ...rows].map(row =>
      row.map(value => this.escapeCsvValue(value)).join(',')
    );

    return `\uFEFF${lines.join('\r\n')}\r\n`;
  }

  downloadCsv(filename: string, headers: string[], rows: Array<Array<string | number>>): void {
    const csv = this.buildCsv(headers, rows);
    const blob = new Blob([csv], { type: 'text/csv;charset=utf-8;' });
    const url = URL.createObjectURL(blob);
    const anchor = document.createElement('a');

    anchor.href = url;
    anchor.download = filename;
    anchor.click();

    URL.revokeObjectURL(url);
  }

  private escapeCsvValue(value: string | number): string {
    const text = String(value ?? '');
    return /[",\r\n]/.test(text) ? `"${text.replace(/"/g, '""')}"` : text;
  }
}
