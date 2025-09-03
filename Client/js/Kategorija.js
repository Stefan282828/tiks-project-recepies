const apiBase = "http://localhost:5121/Kategorija"; // promeni port ako je drugi

// Učitaj sve kategorije
async function loadKategorije() {
  const res = await fetch(`${apiBase}/VratiSve`);
  const data = await res.json();

  const lista = document.getElementById("kategorijaLista");
  lista.innerHTML = "";

  data.forEach(k => {
    const li = document.createElement("li");
    const encodedName = encodeURIComponent(k.naziv);
    li.innerHTML = `
      ${k.naziv}
      <span>
        <button onclick="izmeniKategoriju(${k.id})">✏️</button>
        <button data-action="subcategories" onclick="window.location.href='Podkategorija.html?categoryId=${k.id}&categoryName=${encodedName}'">➕</button>
        <button onclick="obrisiKategoriju(${k.id})">🗑️</button>
      </span>
    `;
    lista.appendChild(li);
  });
}

// Dodaj kategoriju
document.getElementById("kategorijaForm").addEventListener("submit", async e => {
  e.preventDefault();
  const naziv = document.getElementById("naziv").value;

  await fetch(`${apiBase}/Dodaj`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ naziv })
  });

  document.getElementById("naziv").value = "";
  loadKategorije();
});

// Izmeni kategoriju
async function izmeniKategoriju(id) {
  const noviNaziv = prompt("Unesi novi naziv kategorije:");
  if (!noviNaziv) return;

  await fetch(`${apiBase}/Izmeni/${id}`, {
    method: "PUT",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ naziv: noviNaziv })
  });

  loadKategorije();
}

// Obrisi kategoriju
async function obrisiKategoriju(id) {
  if (!confirm("Da li si siguran da želiš obrisati ovu kategoriju?")) return;

  await fetch(`${apiBase}/Obrisi/${id}`, { method: "DELETE" });
  loadKategorije();
}

// Kad se stranica učita
window.onload = loadKategorije;
