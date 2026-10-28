import { OperationalListState } from './operational-list-state';

describe('OperationalListState', () => {
  interface Item { name: string; status: string; }
  let state: OperationalListState<Item>;
  const items = Array.from({ length: 21 }, (_, index) => ({ name: `Item ${index + 1}`, status: index % 2 === 0 ? 'Ativo' : 'Inativo' }));

  beforeEach(() => state = new OperationalListState<Item>());

  it('deve filtrar e voltar para a primeira página', () => {
    state.currentPage = 2;
    state.setFilter('item 2');
    expect(state.currentPage).toBe(1);
    expect(state.getFiltered(items, ['name'])).toHaveSize(3);
  });

  it('deve paginar em dez itens por página', () => {
    expect(state.getVisible(items, ['name'])).toHaveSize(10);
    state.next(items, ['name']);
    expect(state.currentPage).toBe(2);
    expect(state.getVisible(items, ['name'])).toHaveSize(10);
    state.next(items, ['name']);
    expect(state.currentPage).toBe(3);
    expect(state.getVisible(items, ['name'])).toHaveSize(1);
  });
});
