const apiBase = "http://localhost:5121/Kategorija"; // promeni port ako je drugi

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

// Učitaj sve kategorije
async function loadKategorije() {
  const lista = document.getElementById("kategorijaLista");
  lista.innerHTML = "";
  try {
    const res = await safeFetch(`${apiBase}/VratiSve`);
    if (!res) { const li = document.createElement('li'); li.textContent = 'Ne mogu da učitam kategorije (backend nedostupan).'; lista.appendChild(li); return; }
    const data = await res.json();

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
      try { localStorage.setItem('lastCategory', JSON.stringify({ id: k.id, name: k.naziv })); } catch {}
      window.location.href = `Podkategorija.html?categoryId=${k.id}&categoryName=${encodedName}`;
    });
      lista.appendChild(li);
    });
  } catch (err) {
    const li = document.createElement('li');
    li.textContent = 'Ne mogu da učitam kategorije (backend nedostupan).';
    lista.appendChild(li);
  }
}

// Dodaj kategoriju
document.getElementById("kategorijaForm").addEventListener("submit", async e => {
  e.preventDefault();
  const naziv = document.getElementById("naziv").value.trim();
  if (!naziv) return;
  try {
    const r = await safeFetch(`${apiBase}/Dodaj`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ naziv })
    });

    if (!r) return;
    document.getElementById("naziv").value = "";
    loadKategorije();
  } catch (err) {
    console.error(err);
  }
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

    try {
      const r = await safeFetch(`${apiBase}/Izmeni/${id}`, {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ naziv: noviNaziv })
      });
      if (!r) { cancel(); return; }
      await loadKategorije();
    } catch (err) {
      console.error(err);
      cancel();
    }
  });
}

// Obrisi kategoriju
async function obrisiKategoriju(id) {
  if (!confirm("Da li si siguran da želiš obrisati ovu kategoriju?")) return;

  try {
    const r = await safeFetch(`${apiBase}/Obrisi/${id}`, { method: "DELETE" });
    if (!r) return;
    loadKategorije();
  } catch (err) {
    console.error(err);
  }
}

// Offline helper
function setOffline(show) {
  const notice = document.getElementById('offlineNotice');
  if (notice) notice.style.display = show ? 'block' : 'none';
  const form = document.getElementById('kategorijaForm');
  if (form) Array.from(form.querySelectorAll('input,button')).forEach(el => el.disabled = !!show);
}

// Kad se stranica učita
window.onload = loadKategorije;
