const apiBasePod = "http://localhost:5121/Podkategorija";
const apiBaseKat = "http://localhost:5121/Kategorija";

function getQueryParam(name) {
  return new URLSearchParams(window.location.search).get(name);
}

const kategorijaId = parseInt(getQueryParam("categoryId"), 10);
const kategorijaNazivParam = getQueryParam("categoryName");

async function safeFetch(url, opts) {
  try {
    const res = await fetch(url, opts);
    if (!res.ok) {
      console.warn('Server returned non-ok', res.status);
      return null;
    }
    return res;
  } catch (err) {
    console.error('Fetch error:', err);
    setOffline(true);
    return null;
  }
}

async function ensureKategorijaHeader() {
  const titleEl = document.getElementById("kategorijaNaziv");
  if (kategorijaNazivParam) {
    titleEl.textContent = kategorijaNazivParam;
    return;
  }
  const res = await safeFetch(`${apiBaseKat}/${kategorijaId}`);
  if (!res) { titleEl.textContent = `Kategorija #${kategorijaId}`; return; }
  const data = await res.json();
  titleEl.textContent = data.naziv || `Kategorija #${kategorijaId}`;
}

async function loadPodkategorije() {
  const lista = document.getElementById("podkategorijaLista");
  lista.innerHTML = "";
  const res = await safeFetch(`${apiBasePod}/ZaKategoriju/${kategorijaId}`);
  if (!res) { const li = document.createElement('li'); li.textContent = 'Ne mogu da učitam podkategorije (backend nedostupan).'; lista.appendChild(li); return; }
  const data = await res.json();

  data.forEach(p => {
    const li = document.createElement("li");
    li.dataset.id = p.id;
    const encodedName = encodeURIComponent(p.naziv);
    li.classList.add('clickable-item');
    li.innerHTML = `
      <span class=\"item-title\">${p.naziv}</span>
      <span class=\"item-actions\">
        <button onclick=\"event.stopPropagation(); window.izmeniPodkategoriju(${p.id}, this)\">✏️</button>
        <button onclick=\"event.stopPropagation(); window.obrisiPodkategoriju(${p.id})\">🗑️</button>
      </span>
    `;
    li.addEventListener('click', () => {
      window.location.href = `Recept.html?podId=${p.id}&podName=${encodedName}`;
    });
    lista.appendChild(li);
  });
}

// Dodaj podkategoriju
const form = document.getElementById("podkategorijaForm");
form.addEventListener("submit", async (e) => {
  e.preventDefault();
  const naziv = document.getElementById("podNaziv").value.trim();
  if (!naziv) return;
  try {
    const r = await safeFetch(`${apiBasePod}/Dodaj`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ naziv, kategorijaId })
    });
    if (!r) return;
    document.getElementById("podNaziv").value = "";
    loadPodkategorije();
  } catch (err) {
    console.error(err);
  }
});

// Izmeni podkategoriju
async function izmeniPodkategoriju(id, btnEl) {
  const li = btnEl.closest('li');
  const title = li.querySelector('.item-title');
  const current = title.textContent.trim();

  const editor = document.createElement('div');
  editor.className = 'inline-row';
  editor.innerHTML = `
    <input class=\"inline-input\" type=\"text\" value=\"${current.replace(/\"/g, '&quot;')}\" />
    <button class=\"inline-save\">Sačuvaj</button>
    <button class=\"inline-cancel\">Otkaži</button>
  `;

  title.replaceWith(editor);

  const input = editor.querySelector('.inline-input');
  input.focus();

  const cancel = () => {
    editor.replaceWith(title);
  };

  editor.querySelector('.inline-cancel').addEventListener('click', (e) => {
    e.stopPropagation();
    cancel();
  });

  editor.querySelector('.inline-save').addEventListener('click', async (e) => {
    e.stopPropagation();
    const noviNaziv = input.value.trim();
    if (!noviNaziv || noviNaziv === current) { cancel(); return; }

    try {
      const r = await safeFetch(`${apiBasePod}/Izmeni/${id}`, {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ naziv: noviNaziv, kategorijaId })
      });
      if (!r) { cancel(); return; }
      await loadPodkategorije();
    } catch (err) {
      console.error(err);
      cancel();
    }
  });
}

// Obrisi podkategoriju
async function obrisiPodkategoriju(id) {
  if (!confirm("Da li si siguran da želiš obrisati ovu podkategoriju?")) return;
  try {
    const r = await safeFetch(`${apiBasePod}/Obrisi/${id}`, { method: "DELETE" });
    if (!r) return;
    loadPodkategorije();
  } catch (err) {
    console.error(err);
  }
}

window.izmeniPodkategoriju = izmeniPodkategoriju;
window.obrisiPodkategoriju = obrisiPodkategoriju;

function setOffline(show) {
  const notice = document.getElementById('offlineNotice');
  if (notice) notice.style.display = show ? 'block' : 'none';
  const form = document.getElementById('podkategorijaForm');
  if (form) Array.from(form.querySelectorAll('input,button')).forEach(el => el.disabled = !!show);
}

window.addEventListener("load", async () => {
  // If missing id, just render empty without alerts
  if (!Number.isFinite(kategorijaId)) {
    await ensureKategorijaHeader();
    return;
  }
  await ensureKategorijaHeader();
  await loadPodkategorije();
});
