const apiBasePod = "http://localhost:5121/Podkategorija";
const apiBaseKat = "http://localhost:5121/Kategorija";

function getQueryParam(name) {
  return new URLSearchParams(window.location.search).get(name);
}

const kategorijaId = parseInt(getQueryParam("categoryId"), 10);
const kategorijaNazivParam = getQueryParam("categoryName");

async function ensureKategorijaHeader() {
  const titleEl = document.getElementById("kategorijaNaziv");
  if (kategorijaNazivParam) {
    titleEl.textContent = kategorijaNazivParam;
    return;
  }
  try {
    const res = await fetch(`${apiBaseKat}/${kategorijaId}`);
    if (res.ok) {
      const data = await res.json();
      titleEl.textContent = data.naziv || `Kategorija #${kategorijaId}`;
    }
  } catch (_) {
    titleEl.textContent = `Kategorija #${kategorijaId}`;
  }
}

async function loadPodkategorije() {
  const lista = document.getElementById("podkategorijaLista");
  lista.innerHTML = "";
  const res = await fetch(`${apiBasePod}/ZaKategoriju/${kategorijaId}`);
  const data = res.ok ? await res.json() : [];

  data.forEach(p => {
    const li = document.createElement("li");
    li.dataset.id = p.id;
    li.innerHTML = `
      <span class=\"item-title\">${p.naziv}</span>
      <span class=\"item-actions\">
        <button onclick=\"window.izmeniPodkategoriju(${p.id}, this)\">✏️</button>
        <button onclick=\"window.obrisiPodkategoriju(${p.id})\">🗑️</button>
      </span>
    `;
    lista.appendChild(li);
  });
}

// Dodaj podkategoriju
const form = document.getElementById("podkategorijaForm");
form.addEventListener("submit", async (e) => {
  e.preventDefault();
  const naziv = document.getElementById("podNaziv").value.trim();
  if (!naziv) return;
  await fetch(`${apiBasePod}/Dodaj`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ naziv, kategorijaId })
  });
  document.getElementById("podNaziv").value = "";
  loadPodkategorije();
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

    await fetch(`${apiBasePod}/Izmeni/${id}`, {
      method: "PUT",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ naziv: noviNaziv, kategorijaId })
    });
    await loadPodkategorije();
  });
}

// Obrisi podkategoriju
async function obrisiPodkategoriju(id) {
  if (!confirm("Da li si siguran da želiš obrisati ovu podkategoriju?")) return;
  await fetch(`${apiBasePod}/Obrisi/${id}`, { method: "DELETE" });
  loadPodkategorije();
}

window.izmeniPodkategoriju = izmeniPodkategoriju;
window.obrisiPodkategoriju = obrisiPodkategoriju;

window.addEventListener("load", async () => {
  if (!Number.isFinite(kategorijaId)) {
    alert("Nedostaje categoryId u URL-u");
    return;
  }
  await ensureKategorijaHeader();
  await loadPodkategorije();
});
