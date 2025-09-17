// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Shared helpers
function formatEur(value) {
  try {
    return new Intl.NumberFormat('fr-FR', { style: 'currency', currency: 'EUR' }).format(value);
  } catch (err) {
    const num = Number(value);
    if (!Number.isFinite(num)) {
      return '0,00 \u20ac';
    }
    return num.toFixed(2).replace('.', ',') + ' \u20ac';
  }
}

function applyCartSummary(summary) {
  if (!summary) return;
  const nav = document.querySelector('a[aria-label="Cart"]');
  if (!nav) return;
  const badge = nav.querySelector('.cart-count, .badge');
  const uniqueValue = typeof summary.uniqueItems === 'number'
    ? summary.uniqueItems
    : (typeof summary.totalItems === 'number' ? summary.totalItems : 0);
  if (badge) {
    badge.textContent = String(uniqueValue);
  }
  const totalSum = summary.totalSum;
  if (typeof totalSum !== 'undefined') {
    const sumElement = nav.querySelector('.cart-total, .ms-1.text-muted');
    if (sumElement) {
      sumElement.textContent = formatEur(totalSum);
    }
  }
}

// Intercept add-to-cart links to avoid jump-to-top reloads
(function () {
  document.addEventListener('click', async (event) => {
    const link = event.target.closest('a.js-add-to-cart');
    if (!link) return;
    if (event.defaultPrevented || event.button !== 0 || event.metaKey || event.ctrlKey || event.shiftKey || event.altKey) {
      return;
    }

    event.preventDefault();
    if (link.dataset.cartPending === '1') return;
    link.dataset.cartPending = '1';

    try {
      const response = await fetch(link.href, { headers: { Accept: 'application/json' } });
      if (!response.ok) throw new Error('HTTP ' + response.status);
      const data = await response.json();
      applyCartSummary(data);
      link.classList.add('cart-action-done');
      setTimeout(() => link.classList.remove('cart-action-done'), 600);
    } catch (err) {
      console.error('Add to cart failed', err);
      window.location.assign(link.href);
    } finally {
      delete link.dataset.cartPending;
    }
  });
})();

// Catalog endless scrolling
(function () {
  const grid = document.getElementById('catalog-grid');
  const btn = document.getElementById('load-more');
  if (!grid || !btn) return;

  function highlight(text, query) {
    if (!text || !query) return text || '';
    try {
      const esc = query.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
      const re = new RegExp(esc, 'gi');
      return text.replace(re, (m) => `<mark>${m}</mark>`);
    } catch {
      return text;
    }
  }

  // Localized labels
  const loadMoreLabel = grid.dataset.loadmoreLabel || 'Load more';
  const loadingLabel = grid.dataset.loadingLabel || 'Loading...';
  const allLoadedLabel = grid.dataset.allloadedLabel || 'All items loaded';

  async function loadMore() {
    const page = parseInt(grid.dataset.page || '1', 10) + 1;
    const pageSize = parseInt(grid.dataset.pagesize || '6', 10);
    const total = parseInt(grid.dataset.total || '0', 10);
    const loaded = grid.querySelectorAll('.product-card').length;
    if (loaded >= total) {
      finish();
      return;
    }

    const q = grid.dataset.q || '';
    const sort = grid.dataset.sort || '';
    const minPrice = grid.dataset.minprice || '';
    const maxPrice = grid.dataset.maxprice || '';

    const params = new URLSearchParams();
    if (q) params.set('q', q);
    if (sort) params.set('sort', sort);
    if (minPrice) params.set('minPrice', minPrice);
    if (maxPrice) params.set('maxPrice', maxPrice);
    params.set('page', String(page));
    params.set('pageSize', String(pageSize));

    btn.disabled = true;
    btn.textContent = loadingLabel;
    try {
      const res = await fetch('/api/products?' + params.toString(), { headers: { Accept: 'application/json' } });
      if (!res.ok) throw new Error('HTTP ' + res.status);
      const items = await res.json();
      appendItems(items, q);
      grid.dataset.page = String(page);
      const newLoaded = grid.querySelectorAll('.product-card').length;
      if (newLoaded >= total || items.length < pageSize) finish();
    } catch (e) {
      console.error('Load more failed', e);
      finish();
    } finally {
      if (!btn.hasAttribute('data-finished')) {
        btn.disabled = false;
        btn.textContent = loadMoreLabel;
      }
    }
  }

  function finish() {
    btn.setAttribute('data-finished', '1');
    btn.classList.add('disabled');
    btn.textContent = allLoadedLabel;
  }

  function appendItems(items, q) {
    const priceLabel = grid.dataset.priceLabel || 'Price';
    const detailsLabel = grid.dataset.detailsLabel || 'Details';
    const addToCartLabel = grid.dataset.addtocartLabel || 'Add to cart';
    const frag = document.createDocumentFragment();
    const returnUrl = encodeURIComponent(location.pathname + location.search);
    for (const s of items) {
      const col = document.createElement('div');
      col.className = 'col-12 col-sm-6 col-lg-4 mb-4 product-card';
      col.innerHTML = `
        <div class="card h-100">
          <a href="/Products/Details/${s.id}">
            <img src="${s.imageUrl}" class="card-img-top product-card-img" alt="${escapeHtml(s.name || '')}">
          </a>
          <div class="card-body d-flex flex-column">
            <h5 class="card-title">${highlight(escapeHtml(s.name || ''), q)}</h5>
            <p class="card-text">${highlight(escapeHtml(s.description || ''), q)}</p>
            <p class="card-text mt-auto"><strong>${priceLabel}: ${formatEur(s.price)}</strong></p>
            <div class="product-card-actions">
              <a href="/Products/Details/${s.id}" class="btn btn-outline-secondary w-100">${detailsLabel}</a>
              <a href="/Cart/AddToCart/${s.id}?returnUrl=${returnUrl}" class="btn btn-primary w-100 js-add-to-cart">${addToCartLabel}</a>
            </div>
          </div>
        </div>`;
      frag.appendChild(col);
    }
    grid.appendChild(frag);
  }

  function escapeHtml(str) {
    return (str || '').replace(/[&<>"']/g, (c) => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' }[c]));
  }

  btn.addEventListener('click', loadMore);
})();

// Cart page AJAX quantity updates
(function () {
  const table = document.getElementById('cart-table');
  if (!table) return;

  function updateRow(row, quantity, subtotal) {
    const qtyInput = row.querySelector('input[name="quantity"]');
    if (qtyInput) qtyInput.value = String(quantity);
    const subtotalCell = row.querySelector('[data-subtotal]');
    if (subtotalCell) subtotalCell.textContent = formatEur(subtotal);
  }

  function removeRow(row) {
    row.parentElement.removeChild(row);
  }

  function updateTotal(total) {
    const totalCell = document.getElementById('cart-total-sum');
    if (totalCell) totalCell.textContent = formatEur(total);
  }

  async function fetchJson(url, options) {
    const res = await fetch(url, Object.assign({ headers: { Accept: 'application/json' } }, options || {}));
    if (!res.ok) throw new Error('HTTP ' + res.status);
    return await res.json();
  }

  async function postForm(url, data) {
    const body = new URLSearchParams(data);
    return fetchJson(url, { method: 'POST', headers: { 'Content-Type': 'application/x-www-form-urlencoded', Accept: 'application/json' }, body });
  }

  table.addEventListener('click', async (e) => {
    const link = e.target.closest('a[data-action]');
    if (!link) return;
    e.preventDefault();
    const row = e.target.closest('tr[data-id]');
    if (!row) return;
    try {
      const url = link.getAttribute('href');
      const data = await fetchJson(url);
      if (data && data.item) {
        if (data.item.quantity <= 0) {
          removeRow(row);
        } else {
          updateRow(row, data.item.quantity, data.item.subtotal);
        }
      }
      if (data) {
        applyCartSummary(data);
        updateTotal(data.totalSum);
      }
    } catch (err) {
      console.error('Cart update failed', err);
    }
  });

  table.addEventListener('change', async (e) => {
    const input = e.target.closest('input[name="quantity"]');
    if (!input) return;
    const row = e.target.closest('tr[data-id]');
    if (!row) return;
    const id = row.getAttribute('data-id');
    const parsed = parseInt(input.value || '1', 10);
    const q = Number.isNaN(parsed) ? 1 : Math.max(1, parsed);
    input.value = String(q);
    try {
      const data = await postForm('/Cart/SetQuantity', { id, quantity: String(q) });
      if (data && data.item) {
        if (data.item.quantity <= 0) {
          removeRow(row);
        } else {
          updateRow(row, data.item.quantity, data.item.subtotal);
        }
      }
      if (data) {
        applyCartSummary(data);
        updateTotal(data.totalSum);
      }
    } catch (err) {
      console.error('Set quantity failed', err);
    }
  });
})();

// Copy-to-clipboard helper (for order id etc.)
(function () {
  document.addEventListener('click', async (e) => {
    const btn = e.target.closest('.copy-btn[data-copy-target]');
    if (!btn) return;
    e.preventDefault();
    const sel = btn.getAttribute('data-copy-target');
    if (!sel) return;
    const el = document.querySelector(sel);
    if (!el) return;
    const text = (el.textContent || '').trim();
    try {
      if (navigator.clipboard && navigator.clipboard.writeText) {
        await navigator.clipboard.writeText(text);
      } else {
        const ta = document.createElement('textarea');
        ta.value = text;
        ta.setAttribute('readonly', '');
        ta.className = 'position-absolute top-0 start-0 opacity-0 p-0 m-0 border-0';
        document.body.appendChild(ta);
        ta.select();
        document.execCommand('copy');
        document.body.removeChild(ta);
      }
      btn.classList.add('copied');
      setTimeout(() => btn.classList.remove('copied'), 800);
    } catch (err) {
      console.error('Copy failed', err);
    }
  });
})();

// Normalize language dropdown labels (ensure RU shows full name)
(function () {
  document.addEventListener('DOMContentLoaded', () => {
    try {
      const menu = document.getElementById('langDropdown');
      if (!menu) return;
      const ruInput = document.querySelector('#langDropdown + .dropdown-menu form input[name="culture"][value="ru"]');
      if (ruInput) {
        const btn = ruInput.closest('form')?.querySelector('button.dropdown-item');
        if (btn) btn.textContent = 'RU';
      }
      const enInput = document.querySelector('#langDropdown + .dropdown-menu form input[name="culture"][value="en"]');
      if (enInput) {
        const btn = enInput.closest('form')?.querySelector('button.dropdown-item');
        if (btn) btn.textContent = 'EN';
      }
    } catch (e) {
      console.warn('Lang menu normalize failed', e);
    }
  });
})();
