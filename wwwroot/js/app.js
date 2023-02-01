//    <!-- Log in -->
const currentUsername = "";
const currentUserpasswords = "";
var token = "";

const notExist = document.getElementById("notExist");

const userLogin = () => {
  const username = document.getElementById("user-name");
  const userpasswords = document.getElementById("user-password");
  // notExist.style.visibility="hidden";

  var myHeaders = new Headers();
  myHeaders.append("Content-Type", "application/json");

  var raw = JSON.stringify({
    Name: username.value.trim(),
    Password: userpasswords.value.trim(),
  });
  var requestOptions = {
    method: "POST",
    headers: myHeaders,
    body: raw,
    redirect: "follow",
  };
  fetch("https://localhost:7208/User/Login", requestOptions)
    .then((response) => response.text())
    .then((result) => {
      if (result.includes("401")) {
        username.value = "";
        userpasswords.value = "";
        debugger;
        notExist.style.visibility = "visible";
        // alert("לא קיים");
      } else {
        this.token = result.toString();
        sessionStorage.setItem("token", token);
        location.href = "list.html";
      }
    })
    .catch((error) => {
      alert("User not found...");
    });
};

// פונקצית בקרה
function check() {
  token = sessionStorage.getItem("token");
  updateDetails();
  getItems();
  getUsers();
  sessionStorage.removeItem("token");
}
// פונקצית התנתקות
function disconnection() {
  token = "";
  location.href = "index.html";
}
// הצגת פרטי משתמש
function updateDetails() {
  let Dname = document.getElementById("name");
  let Dpassword = document.getElementById("password");

  var myHeaders = new Headers();
  myHeaders.append("Authorization", `Bearer ${token}`);
  myHeaders.append("Content-Type", "application/json");

  var requestOptions = {
    method: "GET",
    headers: myHeaders,
    redirect: "follow",
  };
  debugger;
  fetch("https://localhost:7208/User/GetCurrentUser", requestOptions)
    .then((response) => response.json())
    .then((result) => {
      Dname.innerHTML = result.name;
      Dpassword.innerHTML = result.password;
    })
    .catch((error) => alert("error " + error));
}

//    <!-- User List -->
const uriU = "/User";
let users = [];
const userList = document.getElementById("userList");

function getUsers() {
  var myHeaders = new Headers();
  myHeaders.append("Authorization", `Bearer ${token}`);
  myHeaders.append("Content-Type", "application/json");

  var requestOptions = {
    method: "GET",
    headers: myHeaders,
    redirect: "follow",
  };
  debugger;
  fetch("https://localhost:7208/User/GetAll", requestOptions)
    .then(
      (response) =>
        // {
        // if (response.status == 403) {
        //   alert("403 Forbidden");
        //   return;
        // }
        response.json()
      // ;}
    )

    .then((result) => {
      // if(result==undefined) {
      //   alert("byebye");
      //   return;
      // }
      userList.style.visibility = "visible";
      _displayUsers(result);
    })
    .catch((error) => {
        if (!(error == "SyntaxError: Unexpected end of JSON input"))
          alert("unable to get users... \n" + error);
      }
    );
}

function addUser() {
  const addNameTextbox = document.getElementById("add-user-name");
  const addPasswordTextbox = document.getElementById("add-user-password");
  debugger;
  const user = {
    Name: addNameTextbox.value.trim(),
    Password: addPasswordTextbox.value.trim(),
    IsAdmin: false,
  };
  debugger;
  fetch("/User", {
    method: "POST",
    headers: {
      Accept: "application/json",
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify(user),
  })
    .then((response) => response.json())
    .then(() => {
      getUsers();
      addNameTextbox.value = "";
      addPasswordTextbox.value = "";
    })
    .catch((error) => alert("Unable to add users.. " + error));
}

function deleteUser(idToDelete) {
  var myHeaders = new Headers();
  myHeaders.append("Authorization", `Bearer ${token}`);
  myHeaders.append("Content-Type", "application/json");

  var requestOptions = {
    method: "DELETE",
    headers: myHeaders,
    redirect: "follow",
  };
  fetch(`${uriU}/${idToDelete}`, requestOptions)
    .then((response) => {
      // if the choosen one is admin- 403-forbidden!
      if (response.status == 403) {
        alert("cannot delete an admin");
        return;
      }
      getUsers();
    })
    .catch((error) => {
      alert("error" + error);
    });
}

function _displayCountUsers(itemCount) {
  const name = itemCount <= 1 ? "user" : "users";
  document.getElementById("counterUser").innerText = `${itemCount} ${name}`;
}

function _displayUsers(data) {
  const tBody = document.getElementById("users");
  tBody.innerHTML = "";

  _displayCountUsers(data.length);

  const button = document.createElement("button");

  data.forEach((user) => {
    let isAdminCheckbox = document.createElement("input");
    isAdminCheckbox.type = "checkbox";
    isAdminCheckbox.disabled = true;
    isAdminCheckbox.checked = user.isAdmin;
    let deleteButton = button.cloneNode(false);
    deleteButton.innerText = "Delete";
    deleteButton.setAttribute("onclick", `deleteUser(${user.id})`);
    let tr = tBody.insertRow();
    let td1 = tr.insertCell(0);
    td1.appendChild(isAdminCheckbox);
    let td2 = tr.insertCell(1);
    let textNode = document.createTextNode(user.name);
    td2.appendChild(textNode);
    let td3 = tr.insertCell(2);
    let textNode2 = document.createTextNode(user.password);
    td3.appendChild(textNode2);

    let td4 = tr.insertCell(3);
    td4.appendChild(deleteButton);
  });

  users = data;
}

//    <!-- Task List -->
const uri = "/MyTask";
let tasks = [];

function getItems() {
  var myHeaders = new Headers();
  myHeaders.append("Authorization", `Bearer ${token}`);
  myHeaders.append("Content-Type", "application/json");

  var requestOptions = {
    method: "GET",
    headers: myHeaders,
    redirect: "follow",
  };

  fetch(uri, requestOptions)
    .then((response) => response.json())
    .then((data) => _displayItems(data))
    .catch((error) => alert("Unable to get items... \n" + error));
}

function addItem() {
  const addNameTextbox = document.getElementById("add-name");
  const item = {
    isDone: false,
    name: addNameTextbox.value.trim(),
  };

  fetch(uri, {
    method: "POST",
    headers: {
      Accept: "application/json",
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify(item),
  })
    .then((response) => response.json())
    .then(() => {
      getItems();
      addNameTextbox.value = "";
    })
    .catch((error) => alert("Unable to add item. " + error));
}

function deleteItem(id) {
  fetch(`${uri}/${id}`, {
    method: "DELETE",
    headers: {
      Accept: "application/json",
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    },
  })
    .then(() => getItems())
    .catch((error) => console.error("Unable to delete item.", error));
}

function displayEditForm(id) {
  const item = tasks.find((item) => item.id === id);
  document.getElementById("edit-name").value = item.name;
  document.getElementById("edit-id").value = item.id;
  document.getElementById("edit-userid").value = item.userId;
  document.getElementById("edit-isDone").checked = item.isDone;
  document.getElementById("editForm").style.display = "block";
}

function updateItem() {
  const itemId = document.getElementById("edit-id").value;
  const item = {
    id: parseInt(itemId, 10),
    isdone: document.getElementById("edit-isDone").checked,
    name: document.getElementById("edit-name").value.trim(),
    userid: document.getElementById("edit-userid").value,
  };
  debugger;
  fetch(`${uri}/${itemId}`, {
    method: "PUT",
    headers: {
      Accept: "application/json",
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    },

    body: JSON.stringify(item),
  })
    .then(() => getItems())
    .catch((error) => console.error("Unable to update item.", error));

  closeInput();
  return false;
}

function closeInput() {
  document.getElementById("editForm").style.display = "none";
}

function _displayCount(itemCount) {
  const name = itemCount <= 1 ? "task" : "task kinds";

  document.getElementById("counter").innerText = `${itemCount} ${name}`;
}

function _displayItems(data) {
  const tBody = document.getElementById("tasks");
  tBody.innerHTML = "";

  _displayCount(data.length);

  const button = document.createElement("button");

  data.forEach((item) => {
    debugger;
    let isDoneCheckbox = document.createElement("input");
    isDoneCheckbox.type = "checkbox";
    isDoneCheckbox.disabled = true;
    isDoneCheckbox.checked = item.isDone;

    let editButton = button.cloneNode(false);
    editButton.innerText = "Edit";
    editButton.setAttribute("onclick", `displayEditForm(${item.id})`);

    let deleteButton = button.cloneNode(false);
    deleteButton.innerText = "Delete";
    deleteButton.setAttribute("onclick", `deleteItem(${item.id})`);

    let tr = tBody.insertRow();

    let td1 = tr.insertCell(0);
    td1.appendChild(isDoneCheckbox);

    let td2 = tr.insertCell(1);
    let textNode = document.createTextNode(item.name);
    td2.appendChild(textNode);

    let td3 = tr.insertCell(2);

    td3.appendChild(editButton);

    let td4 = tr.insertCell(3);
    td4.appendChild(deleteButton);
    // }
  });

  tasks = data;
}
