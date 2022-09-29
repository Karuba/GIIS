async function getContacts() {

   const response = await fetch("/api/contacts", {
      method: "GET",
      headers: { "Accept": "application/json" }
   });

   if (response.ok === true) {

      const contacts = await response.json();
      const rows = document.querySelector("tbody");
      rows.innerHTML = '';
      contacts.forEach(contact => rows.append(row(contact)));
   }
};

function download(text, name, type) {
   var a = document.getElementById("a");
   var file = new Blob([text], { type: type });
   a.href = URL.createObjectURL(file);
   a.download = name;
   a.click();
}

document.querySelector("#download-file").addEventListener('click', async function () {
   try {
      let text_data = await downloadFile();
      //document.querySelector("#preview-text").textContent = text_data;
      download(text_data, 'AllContacts.txt', 'text/plain')
   }
   catch (e) {
      alert(e.message);
   }
});

document.querySelector("#download-select-file").addEventListener('click', async function () {
   try {
      let text_data = "";
      let items = document.querySelector("tbody").childNodes;
      for (const i in items) {
         if (items[i].nodeType == 1) {

            console.log(items[i].innerText.split("\t"))
            let row = items[i].innerText.split("\t");
            text_data += "Name: " + row[0] + "; Address: " + row[1] + ";\n";
         }
      }
      //document.querySelector("#preview-text").textContent = text_data;
      download(text_data, 'SelectContacts.txt', 'text/plain')
   }
   catch (e) {
      alert(e.message);
   }
});

async function downloadFile() {
   let response = await fetch("/api/contacts/file");

   if (response.status != 200) {
      throw new Error("Server Error");
   }

   let text_data = await response.text();

   return text_data;
}

async function getContactsByName(name) {
   const response = await fetch(`/api/contacts/${name}`, {
      method: "GET",
      headers: { "Accept": "application/json" }
   })
   if (response.ok === true) {
      const contacts = await response.json();
      const rows = document.querySelector("tbody");
      rows.innerHTML = '';
      contacts.forEach(contact => rows.append(row(contact)));
   }
}

async function getContact(id) {
   const response = await fetch(`/api/contact/${id}`, {
      method: "GET",
      headers: { "Accept": "application/json" }
   });
   if (response.ok === true) {
      const contact = await response.json();
      document.getElementById("contactId").value = contact.id;
      document.getElementById("ContactName").value = contact.name;
      document.getElementById("ContactAddress").value = contact.address;
   }
   else {
      const error = await response.json();
      console.log(error.message);
   }
}

async function createContact(contactName, contactAddress) {
   const response = await fetch("api/contacts", {
      method: "POST",
      headers: { "Accept": "application/json", "Content-Type": "application/json" },
      body: JSON.stringify({
         id: guid(),
         name: contactName,
         address: contactAddress
      })
   });
   if (response.ok === true) {
      const contact = await response.json();
      document.querySelector("tbody").append(row(contact));
   }
   else {
      const error = await response.json();
      alert(error.message);
      console.log(error.message);
   }
}

async function editContact(contactId, contactName, contactAddress) {
   const response = await fetch("api/contacts", {
      method: "PUT",
      headers: { "Accept": "application/json", "Content-Type": "application/json" },
      body: JSON.stringify({
         id: contactId,
         name: contactName,
         address: contactAddress
      })
   });
   if (response.ok === true) {
      const contact = await response.json();
      document.querySelector(`tr[data-rowid='${contact.id}']`).replaceWith(row(contact));
   }
   else {
      const error = await response.json();
      alert(error.message);
      console.log(error.message);
   }
}

async function deleteContact(id) {
   const response = await fetch(`/api/contact/${id}`, {
      method: "DELETE",
      headers: { "Accept": "application/json" }
   });
   if (response.ok === true) {
      const contact = await response.json();
      document.querySelector(`tr[data-rowid='${contact.id}']`).remove();
   }
   else {
      const error = await response.json();
      console.log(error.message);
   }
}

function reset() {
   document.getElementById("contactId").value =
      document.getElementById("ContactName").value =
      document.getElementById("ContactAddress").value = "";
}

document.getElementById("resetBtn").addEventListener("click", () => reset());

document.getElementById("saveBtn").addEventListener("click", async () => {
   const id = document.getElementById("contactId").value;
   const name = document.getElementById("ContactName").value;
   const address = document.getElementById("ContactAddress").value;
   if (name.length < 3 || address.length < 3) {
      alert("Name and Address must contain more than 2 letters")
      return;
   }

   console.log(id)
   if (id !== "") {
      await editContact(id, name, address);
   }
   else {
      await createContact(name, address);
   }
   reset();
});

document.getElementById("findBtn").addEventListener("click", async () => {
   const name = document.getElementById("ContactName").value;
   if (name !== "") {
      await getContactsByName(name);
   }

})

document.getElementById("getAllBtn").addEventListener("click", async () => await getContacts());

function row(contact) {

   const tr = document.createElement("tr");
   tr.setAttribute("data-rowid", contact.id)
   tr.setAttribute("data-rowdel", "delete")

   const nameTd = document.createElement("td");
   nameTd.append(contact.name);
   tr.append(nameTd);

   const addressTd = document.createElement("td");
   addressTd.append(contact.address);
   tr.append(addressTd);

   const linksTd = document.createElement("td");

   const editLink = document.createElement("button");
   editLink.append("Modify");
   console.log(`${contact.id} + ${contact.name}`)
   editLink.addEventListener("click", async () => await getContact(contact.id));
   linksTd.append(editLink);

   const removeLink = document.createElement("button");
   removeLink.append("Remove");
   removeLink.addEventListener("click", async () => await deleteContact(contact.id));
   linksTd.append(removeLink);

   tr.appendChild(linksTd);

   return tr;
}

function guid() {
   return ([1e7] + -1e3 + -4e3 + -8e3 + -1e11).replace(/[018]/g, c =>
      (c ^ crypto.getRandomValues(new Uint8Array(1))[0] & 15 >> c / 4).toString(16)
   );
}



getContacts();