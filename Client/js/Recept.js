const apiBaseRecept = "http://localhost:5121/Recept";

function getParam(n) { return new URLSearchParams(location.search).get(n); }
const podId = parseInt(getParam('podId'), 10);
const podName = getParam('podName');

function setHeader() {
  const el = document.getElementById('podkategorijaNaziv');
  if (podName) el.textContent = decodeURIComponent(podName);
}

async function safeFetch(url, opts) {
  try {
    const res = await fetch(url, opts);
    if (!res.ok) {
      console.warn('Server returned non-ok', res.status);
      return null;
    }
    // return Response
    return res;
  } catch (e) {
    console.error('Network error', e);
    setOffline(true);
    return null;
  }
}

function setOffline(show) {
  const notice = document.getElementById('offlineNotice');
  if (notice) notice.classList.toggle('show', !!show);
  const form = document.getElementById('receptForm');
  if (form) {
    Array.from(form.querySelectorAll('input,textarea,button')).forEach(el => {
      (el).disabled = !!show;
    });
  }
}

async function loadRecepti() {
  const lista = document.getElementById('receptLista');
  lista.innerHTML = '';
  try {
    const res = await safeFetch(`${apiBaseRecept}/ZaPodkategoriju/${podId}`);
    const data = await res.json();

    data.forEach(r => {
      const li = document.createElement('li');
      li.dataset.id = r.id;
      li.classList.add('clickable-item');
      li.innerHTML = `
        <span class="item-title"><strong>${r.naziv}</strong> · ${r.vremePripreme} min</span>
        <span class="item-actions">
          <button onclick="event.stopPropagation(); window.editRecept(${r.id}, this)">✏️</button>
          <button onclick="event.stopPropagation(); window.deleteRecept(${r.id})">🗑️</button>
        </span>
      `;
      lista.appendChild(li);
    });
  } catch (e) {
    const li = document.createElement('li');
    li.textContent = 'Ne mogu da učitam recepte (backend nedostupan).';
    lista.appendChild(li);
  }
}

// create
const form = document.getElementById('receptForm');
form.addEventListener('submit', async (e) => {
  e.preventDefault();
  const naziv = document.getElementById('naziv').value.trim();
  const vreme = parseInt(document.getElementById('vreme').value, 10) || 0;
  const opis = document.getElementById('opis').value.trim();
  const uputstvo = document.getElementById('uputstvo').value.trim();
  if (!naziv) return;
  try {
    await safeFetch(`${apiBaseRecept}/Dodaj`, {
      method: 'POST', headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ naziv, opis, vremePripreme: vreme, uputstvoPripreme: uputstvo, podKategorijaId: podId })
    });
    form.reset();
    loadRecepti();
  } catch {}
});

// inline edit
async function editRecept(id, btn) {
  const li = btn.closest('li');
  const currentTitle = li.querySelector('.item-title');
  const nameMatch = currentTitle.textContent.trim();

  const editor = document.createElement('div');
  editor.className = 'recipe-inline';
  editor.innerHTML = `
    <input class="edit-name" type="text" value="${nameMatch.replace(/"/g, '&quot;')}">
    <input class="edit-time" type="number" placeholder="min">
    <div>
      <button class="inline-save">Sačuvaj</button>
      <button class="inline-cancel">Otkaži</button>
    </div>
  `;
  currentTitle.replaceWith(editor);

  editor.querySelector('.inline-cancel').addEventListener('click', (e) => {
    e.stopPropagation();
    editor.replaceWith(currentTitle);
  });

  editor.querySelector('.inline-save').addEventListener('click', async (e) => {
    e.stopPropagation();
    const noviNaziv = editor.querySelector('.edit-name').value.trim();
    const novoVreme = parseInt(editor.querySelector('.edit-time').value, 10) || 0;
    if (!noviNaziv) { editor.replaceWith(currentTitle); return; }
    try {
      await safeFetch(`${apiBaseRecept}/Izmeni/${id}`, {
        method: 'PUT', headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ naziv: noviNaziv, opis: '', vremePripreme: novoVreme, uputstvoPripreme: '', podKategorijaId: podId })
      });
      await loadRecepti();
    } catch {}
  });
}

// delete
async function deleteRecept(id) {
  if (!confirm('Obrisati recept?')) return;
  try {
    await safeFetch(`${apiBaseRecept}/Obrisi/${id}`, { method: 'DELETE' });
    loadRecepti();
  } catch {}
}

window.editRecept = editRecept;
window.deleteRecept = deleteRecept;

window.addEventListener('load', async () => {
  // set back link with params to avoid missing-id alerts when returning
  const back = document.querySelector('.header-link');
  if (back) {
    const href = `Podkategorija.html?categoryId=${getParam('categoryId') || ''}&categoryName=${encodeURIComponent(getParam('categoryName') || '')}`;
    back.setAttribute('href', href);
  }
  setHeader();
  if (Number.isFinite(podId)) {
    loadRecepti();
  }
});
