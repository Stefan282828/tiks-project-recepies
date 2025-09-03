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
    li.innerHTML = `
      ${p.naziv}
      <span>
        <button onclick="window.izmeniPodkategoriju(${p.id})">✏️</button>
        <button onclick="window.obrisiPodkategoriju(${p.id})">🗑️</button>
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
async function izmeniPodkategoriju(id) {
  const noviNaziv = prompt("Unesi novi naziv podkategorije:");
  if (!noviNaziv) return;
  await fetch(`${apiBasePod}/Izmeni/${id}`, {
    method: "PUT",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ naziv: noviNaziv, kategorijaId })
  });
  loadPodkategorije();
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
