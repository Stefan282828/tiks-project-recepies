document.addEventListener('DOMContentLoaded', function () {
    const receptList = document.getElementById('receptList');
    const createForm = document.getElementById('createForm');
    const updateForm = document.getElementById('updateForm');
    const deleteForm = document.getElementById('deleteForm');
    const sastojakForm = document.getElementById('sastojakForm');
    fetchSastojke()

    async function displayReceptList() {
        receptList.innerHTML = '';
        const recepti = await getRecepti();
    
        if (receptList) {
            recepti.forEach(recept => {
                const li = document.createElement('li');
                
                const nazivElement = document.createElement('span');
                nazivElement.textContent = recept.naziv;
                nazivElement.classList.add('bold-naziv');
                
                li.appendChild(nazivElement);
                li.innerHTML += ` - ${recept.opis} - ${recept.kategorija} - ${recept.uputstvoPripreme}`;
                
                receptList.appendChild(li);
            });
        }
    }

    async function getRecepti() {
        const response = await fetch('http://localhost:5121/Recept/VratiRecepte');
        const data = await response.json();
        return data;
    }

    createForm.addEventListener('submit', async function (event) {
        event.preventDefault();
        const formData = new FormData(createForm);
        const receptData = {
            Naziv: formData.get('naziv'),
            Opis: formData.get('opis'),
            VremePr: formData.get('vremePr'),
            Kategorija: formData.get('kategorija'),
            UputstvoPripreme: formData.get('uputstvoPr')
        };
        await createRecept(receptData);
        await displayReceptList();
        createForm.reset();
    });

    async function createRecept(receptData) {
        const response = await fetch(`http://localhost:5121/Recept/DodajRecept`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(receptData),
        });

        if (response.ok) {
            const data = await response.text();
            console.log('Recept created:', data);
        } else {
            console.error('Error creating recept:', response.statusText);
        }
    }



    async function fetchReceptDetails() {
        const receptName = document.getElementById('updateNazivInput').value;
        const recept = await ReceptPoNazivu(receptName);
    
        console.log(recept); 
    
        if (recept) {
            console.log('Setting values...'); 
    
            document.getElementById('updateOpis').value = recept.opis || '';
            console.log('Opis set:', document.getElementById('updateOpis').value); 
    
            document.getElementById('updateKategorija').value = recept.kategorija || '';
            console.log('Kategorija set:', document.getElementById('updateKategorija').value); 
    
            document.getElementById('updateUputstvo').value = recept.uputstvoPripreme || '';
            console.log('Uputstvo Pripreme set:', document.getElementById('updateUputstvo').value); 
        } else {
            console.error(`Recept with name ${receptName} not found.`);
        }
    }
    
    document.getElementById('fetchRecept').addEventListener('click', fetchReceptDetails);
    
    async function ReceptPoNazivu(name) {
        const response = await fetch(`http://localhost:5121/Recept/VratiReceptPoNazivu/${name}`);
        if (response.ok) {
            return await response.json();
        } else {
            console.error(`Failed to get Recept by name ${name}.`);
            return null;
        }
    }

    updateForm.addEventListener('submit', async function (event) {
        event.preventDefault();
        const formData = new FormData(updateForm);
        const receptName = document.getElementById('updateNazivInput').value;
        
        const recept = await ReceptPoNazivu(receptName);
    
        if (!recept) {
            console.error(`Recept with name ${receptName} not found.`);
            return;
        }
    
        const updatedData = {
            Opis: document.getElementById('updateOpis').value,
            Kategorija: document.getElementById('updateKategorija').value,
            UputstvoPripreme: document.getElementById('updateUputstvo').value
        };
    
        await updateRecept(recept.naziv, updatedData);
        await displayReceptList();
        updateForm.reset();
    });

    async function updateRecept(name, updatedData) {
        
        const recept = await ReceptPoNazivu(name);
        if (!recept) {
            console.error(`Recept with name ${name} not found.`);
            return;
        }
        const updatedReceptData = { ...recept, ...updatedData };
        const response = await fetch(`http://localhost:5121/Recept/UpdateRecept/${recept.naziv}`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(updatedReceptData),
        });
        const data = await response.json();
        console.log('Recept updated:', data);
    }




    deleteForm.addEventListener('submit', async function (event) {
        event.preventDefault();
        const receptName = document.getElementById('deleteNaziv').value;

        const recept = await ReceptPoNazivu(receptName);

        if (!recept) {
            console.error(`Recept with name ${receptName} not found.`);
            return;
        }

        await deleteRecept(recept.naziv);
        await displayReceptList();
        deleteForm.reset();
    });

    async function deleteRecept(name) {
        const recept = await ReceptPoNazivu(name);
        if (!recept) {
            console.error(`Recept with name ${name} not found.`);
            return;
        }
        const response = await fetch(`http://localhost:5121/Recept/ObrisiRecept/${recept.naziv}`, {
            method: 'DELETE',
        });

        if (response.ok) {
            console.log(`Recept ${name} deleted successfully.`);
        } else {
            console.error(`Failed to delete Recept ${name}.`);
        }
    }

    async function fetchSastojke()
    {
        fetch('http://localhost:5121/Sastojak/VratiSastojke')
        .then(response => response.json())
        .then(data => {
            const sastojciSelect = document.getElementById('sastojci');
            data.forEach(sastojak => {
                const option = document.createElement('option');
                option.value = sastojak.id; 
                option.text = `${sastojak.naziv}`; 
                sastojciSelect.appendChild(option);
            });
        });
    }

    sastojakForm.addEventListener('submit', async function (event) {
        event.preventDefault();
        const formData = new FormData(sastojakForm);
        const receptData = {
            Naziv: formData.get('naziv')
        };

    
        console.log(receptData.Naziv);
        const recept = await ReceptPoNazivu(receptData.Naziv);
        console.log(recept)
    
        if (!recept) {
            console.error(`Recept with name ${receptData.Naziv} not found or missing ID.`);
            return;
        }
    
        const selectedSastojak = Array.from(document.getElementById('sastojci').selectedOptions)
        .map(option => option.value);

        console.log('Selected Sastojak:', selectedSastojak);

    
        fetch(`http://localhost:5121/Recept/dodaj-sastojak-receptu/${recept.Naziv}/${selectedSastojak.join(',')}`, {
            method: 'POST'
        })
        .then(response => {
            if (response.ok) {
                console.log('Sastojak uspešno dodat receptu');
                
            } else {
                console.error('Greška prilikom dodavanja sastojaka receptu');
            }
        });
    });
    
    
    
    


    // ... (ostatak koda koji nije promenjen)

    // Initial display of Recept list
    displayReceptList();
});
