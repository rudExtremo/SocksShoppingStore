// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

(function(){
  const grid = document.getElementById('catalog-grid');
  const btn = document.getElementById('load-more');
  if (!grid || !btn) return;

  function highlight(text, query){
    if (!text || !query) return text || '';
    try {
      const esc = query.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
      const re = new RegExp(esc, 'gi');
      return text.replace(re, (m)=>`<mark>${m}</mark>`);
    } catch { return text; }
  }

  async function loadMore(){
    const page = parseInt(grid.dataset.page || '1', 10) + 1;
    const pageSize = parseInt(grid.dataset.pagesize || '6', 10);
    const total = parseInt(grid.dataset.total || '0', 10);
    const loaded = grid.querySelectorAll('.product-card').length;
    if (loaded >= total) { finish(); return; }

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

    btn.disabled = true; btn.textContent = 'Loading...';
    try {
      const res = await fetch('/api/products?' + params.toString(), { headers: { 'Accept': 'application/json' } });
      if (!res.ok) throw new Error('HTTP ' + res.status);
      const items = await res.json();
      appendItems(items, q);
      grid.dataset.page = String(page);
      const newLoaded = grid.querySelectorAll('.product-card').length;
      if (newLoaded >= total || items.length < pageSize) finish();
    } catch (e){
      console.error('Load more failed', e);
      finish();
    } finally {
      if (!btn.hasAttribute('data-finished')) { btn.disabled = false; btn.textContent = 'Load more'; }
    }
  }

  function finish(){
    btn.setAttribute('data-finished','1');
    btn.classList.add('disabled');
    btn.textContent = 'All items loaded';
  }

  function appendItems(items, q){
    const priceLabel = grid.dataset.priceLabel || 'Price';
    const detailsLabel = grid.dataset.detailsLabel || 'Details';
    const addToCartLabel = grid.dataset.addtocartLabel || 'Add to cart';
    const frag = document.createDocumentFragment();
    for (const s of items){
      const col = document.createElement('div');
      col.className = 'col-md-4 mb-4 product-card';
      col.innerHTML = `
        <div class="card h-100">
          <a href="/Products/Details/${s.id}">
            <img src="${s.imageUrl}" class="card-img-top product-card-img" alt="${escapeHtml(s.name || '')}">
          </a>
          <div class="card-body d-flex flex-column">
            <h5 class="card-title">${highlight(escapeHtml(s.name||''), q)}</h5>
            <p class="card-text">${highlight(escapeHtml(s.description||''), q)}</p>
            <p class="card-text mt-auto"><strong>${priceLabel}: ${formatEur(s.price)}</strong></p>
            <div class="d-flex gap-2">
              <a href="/Products/Details/${s.id}" class="btn btn-outline-secondary">${detailsLabel}</a>
              <a href="/Cart/AddToCart/${s.id}" class="btn btn-primary">${addToCartLabel}</a>
            </div>
          </div>
        </div>`;
      frag.appendChild(col);
    }
    grid.appendChild(frag);
  }

  function escapeHtml(str){
    return (str||'').replace(/[&<>"']/g, c=>({"&":"&amp;","<":"&lt;",">":"&gt;","\"":"&quot;","'":"&#39;"}[c]));
  }

  function formatEur(value){
    try { return new Intl.NumberFormat('fr-FR', { style: 'currency', currency: 'EUR' }).format(value); }
    catch { return (Number(value).toFixed(2) + ' €'); }
  }

  btn.addEventListener('click', loadMore);
})();
