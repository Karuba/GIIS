async function getContacts() {

   const response = await fetch("/api/contacts", {
      method: "GET",
      headers: { "Accept": "application/json" }
   });

   if (response.ok === true) {

      const contacts = await response.json();
      const rows = document.querySelector("tbody");

      contacts.forEach(contact => rows.append(row(contact)));
   }
};

function row(contact) {

   const tr = document.createElement("tr");

   const nameTd = document.createElement("td");
   nameTd.append(contact.name);
   tr.append(nameTd);

   const addressTd = document.createElement("td");
   addressTd.append(contact.address);
   tr.append(addressTd);

   //   const linksTd = document.createElement("td");

   //   const editLink = document.createElement("button");
   //   editLink.append("Modify");
   //   editLink.addEventListener("click", async () => await getUser(contact.id));
   //   linksTd.append(editLink);
   //
   //   const removeLink = document.createElement("button");
   //   removeLink.append("Удалить");
   //   removeLink.addEventListener("click", async () => await deleteUser(contact.id));

   //   linksTd.append(removeLink);
   //   tr.appendChild(linksTd);

   return tr;
}

getContacts();