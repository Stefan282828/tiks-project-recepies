const apiBase = "http://localhost:5121/Kategorija"; // promeni port ako je drugi

// Učitaj sve kategorije
async function loadKategorije() {
  const res = await fetch(`${apiBase}/VratiSve`);
  const data = await res.json();

  const lista = document.getElementById("kategorijaLista");
  lista.innerHTML = "";

  data.forEach(k => {
    const li = document.createElement("li");
    li.dataset.id = k.id;
    const encodedName = encodeURIComponent(k.naziv);
    li.classList.add("clickable-item");
    li.innerHTML = `
      <span class=\"item-title\">${k.naziv}</span>
      <span class=\"item-actions\">
        <button onclick=\"event.stopPropagation(); izmeniKategoriju(${k.id}, this)\">✏️</button>
        <button onclick=\"event.stopPropagation(); obrisiKategoriju(${k.id})\">🗑️</button>
      </span>
    `;
    li.addEventListener('click', () => {
      window.location.href = `Podkategorija.html?categoryId=${k.id}&categoryName=${encodedName}`;
    });
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
async function izmeniKategoriju(id, btnEl) {
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

  // replace title with editor temporarily
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

    await fetch(`${apiBase}/Izmeni/${id}`, {
      method: "PUT",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ naziv: noviNaziv })
    });
    await loadKategorije();
  });
}

// Obrisi kategoriju
async function obrisiKategoriju(id) {
  if (!confirm("Da li si siguran da želiš obrisati ovu kategoriju?")) return;

  await fetch(`${apiBase}/Obrisi/${id}`, { method: "DELETE" });
  loadKategorije();
}

// Kad se stranica učita
window.onload = loadKategorije;
