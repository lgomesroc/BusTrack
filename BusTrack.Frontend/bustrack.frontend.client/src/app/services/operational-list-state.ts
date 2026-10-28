export class OperationalListState<T> {
  filter = '';
  currentPage = 1;
  readonly pageSize = 10;

  setFilter(value: string): void {
    this.filter = value ?? '';
    this.currentPage = 1;
  }

  getFiltered(items: T[], fields: Array<keyof T>): T[] {
    const term = this.filter.trim().toLocaleLowerCase('pt-BR');

    if (!term) {
      return items;
    }

    return items.filter(item =>
      fields.some(field =>
        String(item[field] ?? '')
          .toLocaleLowerCase('pt-BR')
          .includes(term)
      )
    );
  }

  getVisible(items: T[], fields: Array<keyof T>): T[] {
    const filtered = this.getFiltered(items, fields);
    const start = (this.currentPage - 1) * this.pageSize;
    return filtered.slice(start, start + this.pageSize);
  }

  getTotalPages(items: T[], fields: Array<keyof T>): number {
    return Math.max(1, Math.ceil(this.getFiltered(items, fields).length / this.pageSize));
  }

  next(items: T[], fields: Array<keyof T>): void {
    this.currentPage = Math.min(
      this.currentPage + 1,
      this.getTotalPages(items, fields)
    );
  }

  previous(): void {
    this.currentPage = Math.max(1, this.currentPage - 1);
  }

  reset(): void {
    this.filter = '';
    this.currentPage = 1;
  }
}
