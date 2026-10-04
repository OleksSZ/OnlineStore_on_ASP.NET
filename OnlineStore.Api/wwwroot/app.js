const $ = s => document.querySelector(s);
const esc = s => String(s ?? '').replace(/[&<>"']/g, c => ({'&':'&amp;','<':'&lt;','>':'&gt;','"':'&quot;',"'":'&#39;'}[c]));
const money = n => Number(n).toFixed(2) + ' ₴';
const ST = {New:'Новий',Confirmed:'Підтверджено',Packed:'Зібрано',InDelivery:'В дорозі',Delivered:'Доставлено',
            Cancelled:'Скасовано',Planned:'Заплановано',InProgress:'У процесі',Completed:'Завершено'};
const tag = s => `<span class="tag ${s}">${ST[s] || s}</span>`;

async function api(method, url, body) {
  const r = await fetch('/api/' + url, {
    method, headers: {'Content-Type': 'application/json'},
    body: body ? JSON.stringify(body) : undefined
  });
  if (!r.ok) {
    const e = await r.json().catch(() => ({}));
    throw new Error(e.detail || e.title || r.statusText);
  }
  return r.status === 204 ? null : r.json();
}

function toast(msg, bad) {
  const t = $('#toast');
  t.textContent = msg; t.className = bad ? 'err' : ''; t.style.display = 'block';
  clearTimeout(toast.h); toast.h = setTimeout(() => t.style.display = 'none', 3500);
}

const table = (head, rows) => rows.length
  ? `<table><tr>${head.map(h => `<th>${h}</th>`).join('')}</tr>${rows.map(r => `<tr>${r.map(c => `<td>${c}</td>`).join('')}</tr>`).join('')}</table>`
  : '<p class="mut">Поки порожньо</p>';
const btn = (act, id, label, cls = '') => `<button class="sm ${cls}" data-act="${act}" data-id="${id}">${label}</button>`;
const del = (res, id) => `<button class="sm bad" data-act="delete" data-res="${res}" data-id="${id}">Видалити</button>`;
const field = (name, ph, type = 'text') => `<input name="${name}" type="${type}" placeholder="${ph}" required ${type === 'number' ? 'step="any" min="0"' : ''}>`;
const opts = (list, f) => list.map(x => `<option value="${x.id}">${esc(f(x))}</option>`).join('');

const views = {
  async orders() {
    const [orders, customers, products] = await Promise.all([api('GET', 'orders'), api('GET', 'customers'), api('GET', 'products')]);
    const acts = o => ({
      New: btn('order-confirm', o.id, 'Підтвердити') + btn('order-cancel', o.id, 'Скасувати', 'bad'),
      Confirmed: btn('order-pack', o.id, 'Зібрати') + btn('order-cancel', o.id, 'Скасувати', 'bad'),
      Packed: btn('order-cancel', o.id, 'Скасувати', 'bad')
    }[o.status] || '');
    return `<h2>Нове замовлення</h2>
      <form id="order-form">
        <select name="customerId" required>${opts(customers, c => c.name)}</select>
        <div id="lines"><div class="line">
          <select>${opts(products, p => `${p.name} (${p.stock} шт.)`)}</select>
          <input type="number" min="1" value="1"></div></div>
        <button type="button" class="sm" data-act="add-line">+ позиція</button>
        <button>Створити замовлення</button>
      </form>
      <h2>Замовлення</h2>` +
      table(['ID', 'Клієнт', 'Товари', 'Сума', 'Доставка', 'Статус', "Кур'єр", ''],
        orders.map(o => [o.id, esc(o.customerName), o.items.map(i => `${esc(i.productName)} × ${i.quantity}`).join('<br>'),
          money(o.total), o.deliveryCost ? money(o.deliveryCost) : 'безкоштовно', tag(o.status), esc(o.courierName || '—'), acts(o)]));
  },

  async routes() {
    const [routes, couriers, orders] = await Promise.all([api('GET', 'routes'), api('GET', 'couriers/available'), api('GET', 'orders')]);
    const packed = orders.filter(o => o.status === 'Packed');
    const acts = r => r.status === 'Planned'
      ? `<form data-post="routes/${r.id}/orders" class="inline"><select name="orderId" required>${opts(packed, o => `#${o.id} ${o.customerName}`)}</select> <button class="sm">Додати</button></form>` + btn('route-start', r.id, 'Старт')
      : r.status === 'InProgress' ? btn('route-complete', r.id, 'Завершити') : '';
    return `<h2>Новий маршрут</h2>
      <form data-post="routes" class="row"><select name="courierId" required>${opts(couriers, c => c.name)}</select><button>Створити</button></form>
      <h2>Маршрути</h2>` +
      table(['ID', "Кур'єр", 'Замовлення', 'Статус', ''],
        routes.map(r => [r.id, esc(r.courierName), r.orderIds.map(i => '#' + i).join(', ') || '—', tag(r.status), acts(r)]));
  },

  async customers() {
    const list = await api('GET', 'customers');
    return `<h2>Новий клієнт</h2>
      <form data-post="customers" class="row">${field('name', "Ім'я")}${field('phone', 'Телефон')}${field('email', 'Email', 'email')}${field('address', 'Адреса')}<button>Додати</button></form>
      <h2>Клієнти</h2>` +
      table(['ID', "Ім'я", 'Телефон', 'Email', 'Адреса', ''],
        list.map(c => [c.id, esc(c.name), esc(c.phone), esc(c.email), esc(c.address), del('customers', c.id)]));
  },

  async products() {
    const list = await api('GET', 'products');
    return `<h2>Новий товар</h2>
      <form data-post="products" class="row">${field('name', 'Назва')}${field('price', 'Ціна', 'number')}${field('stock', 'Залишок', 'number')}<button>Додати</button></form>
      <h2>Товари</h2>` +
      table(['ID', 'Назва', 'Ціна', 'Залишок', ''],
        list.map(p => [p.id, esc(p.name), money(p.price), p.stock, btn('product-restock', p.id, '+ склад') + del('products', p.id)]));
  },

  async couriers() {
    const list = await api('GET', 'couriers');
    return `<h2>Новий кур'єр</h2>
      <form data-post="couriers" class="row">${field('name', "Ім'я")}${field('phone', 'Телефон')}<button>Додати</button></form>
      <h2>Кур'єри</h2>` +
      table(['ID', "Ім'я", 'Телефон', 'Статус', ''],
        list.map(c => [c.id, esc(c.name), esc(c.phone), c.isAvailable ? '🟢 вільний' : '🔴 зайнятий', del('couriers', c.id)]));
  }
};

const actions = {
  'order-confirm': id => api('POST', `orders/${id}/confirm`),
  'order-pack': id => api('POST', `orders/${id}/pack`),
  'order-cancel': id => api('POST', `orders/${id}/cancel`),
  'route-start': id => api('POST', `routes/${id}/start`),
  'route-complete': id => api('POST', `routes/${id}/complete`),
  'product-restock': id => {
    const q = prompt('Скільки одиниць додати на склад?');
    return q && api('PATCH', `products/${id}/restock`, {quantity: +q});
  }
};

let current = 'orders';
async function show(name) {
  current = name;
  document.querySelectorAll('nav button').forEach(b => b.classList.toggle('on', b.dataset.id === name));
  try { $('#view').innerHTML = await views[name](); }
  catch (err) { toast(err.message, true); }
}

document.addEventListener('click', async e => {
  const b = e.target.closest('button[data-act]');
  if (!b) return;
  const {act, id, res} = b.dataset;
  if (act === 'tab') return show(id);
  if (act === 'add-line') return $('#lines').append($('#lines .line').cloneNode(true));
  try {
    if (act === 'delete') {
      if (!confirm('Видалити запис?')) return;
      await api('DELETE', `${res}/${id}`);
    } else await actions[act](id);
    toast('Готово'); show(current);
  } catch (err) { toast(err.message, true); }
});

document.addEventListener('submit', async e => {
  e.preventDefault();
  const f = e.target;
  try {
    if (f.id === 'order-form') {
      await api('POST', 'orders', {
        customerId: +f.customerId.value,
        items: [...f.querySelectorAll('.line')].map(l => ({
          productId: +l.querySelector('select').value, quantity: +l.querySelector('input').value}))
      });
    } else {
      const body = {};
      for (const [k, v] of new FormData(f))
        body[k] = (f.elements[k].type === 'number' || k.endsWith('Id')) ? Number(v) : v;
      await api('POST', f.dataset.post, body);
    }
    toast('Збережено'); show(current);
  } catch (err) { toast(err.message, true); }
});

show('orders');