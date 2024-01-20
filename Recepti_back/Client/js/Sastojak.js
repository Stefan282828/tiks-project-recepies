// Sastojak.js
document.addEventListener('DOMContentLoaded', function () {
    const sastojakList = document.getElementById('sastojakList');
    const createForm = document.getElementById('createForm');
    const updateForm = document.getElementById('updateForm');
    const deleteForm = document.getElementById('deleteForm');

    fetchSastojke();

    async function displaySastojakList() {
        sastojakList.innerHTML = '';
        const sastojci = await getSastojci();

        if (sastojakList) {
            sastojci.forEach(sastojak => {
                const li = document.createElement('li');

                const nazivElement = document.createElement('span');
                nazivElement.textContent = sastojak.naziv;
                nazivElement.classList.add('bold-naziv');

                li.appendChild(nazivElement);

                sastojakList.appendChild(li);
            });
        }
    }

    async function getSastojci() {
        const response = await fetch('http://localhost:5121/Sastojak/VratiSastojke');
        const data = await response.json();
        return data;
    }

    createForm.addEventListener('submit', async function (event) {
        event.preventDefault();
        const formData = new FormData(createForm);
        const sastojakData = {
            Naziv: formData.get('naziv')
        };
        await createSastojak(sastojakData);
        await displaySastojakList();
        createForm.reset();
    });

    async function createSastojak(sastojakData) {
        const response = await fetch(`http://localhost:5121/Sastojak/DodajSastojak`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(sastojakData),
        });

        if (response.ok) {
            const data = await response.text();
            console.log('Sastojak created:', data);
        } else {
            console.error('Error creating sastojak:', response.statusText);
        }
    }

    updateForm.addEventListener('submit', async function (event) {
        event.preventDefault();
        const formData = new FormData(updateForm);
        const sastojakName = document.getElementById('updateNazivInput').value;

        const sastojak = await VratiSastojakPoNazivu(sastojakName);

        if (!sastojak) {
            console.error(`Sastojak with name ${sastojakName} not found.`);
            return;
        }

        const updatedData = {
            Naziv: document.getElementById('updateNaziv').value
        };

        await updateSastojak(sastojak.naziv, updatedData);
        await displaySastojakList();
        updateForm.reset();
    });

    async function updateSastojak(name, updatedData) {
        const sastojak = await VratiSastojakPoNazivu(name);
        if (!sastojak) {
            console.error(`Sastojak with name ${name} not found.`);
            return;
        }
        const updatedSastojakData = { ...sastojak, ...updatedData };
        const response = await fetch(`http://localhost:5121/Sastojak/AzurirajSastojak/${sastojak.naziv}`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(updatedSastojakData),
        });
        const data = await response.json();
        console.log('Sastojak updated:', data);
    }

    deleteForm.addEventListener('submit', async function (event) {
        event.preventDefault();
        const sastojakName = document.getElementById('deleteNaziv').value;

        const sastojak = await VratiSastojakPoNazivu(sastojakName);

        if (!sastojak) {
            console.error(`Sastojak with name ${sastojakName} not found.`);
            return;
        }

        await deleteSastojak(sastojak.naziv);
        await displaySastojakList();
        deleteForm.reset();
    });

    async function deleteSastojak(name) {
        const sastojak = await VratiSastojakPoNazivu(name);
        if (!sastojak) {
            console.error(`Sastojak with name ${name} not found.`);
            return;
        }
        const response = await fetch(`http://localhost:5121/Sastojak/ObrisiSastojak/${sastojak.naziv}`, {
            method: 'DELETE',
        });

        if (response.ok) {
            console.log(`Sastojak ${name} deleted successfully.`);
        } else {
            console.error(`Failed to delete Sastojak ${name}.`);
        }
    }

    async function fetchSastojke() {
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

    async function VratiSastojakPoNazivu(name) {
        const response = await fetch(`http://localhost:5121/Sastojak/VratiSastojakPoNazivu/${name}`);
        if (response.ok) {
            return await response.json();
        } else {
            console.error(`Failed to get Sastojak by name ${name}.`);
            return null;
        }
    }
 

    // Initial display of Sastojak list
    displaySastojakList();
});
