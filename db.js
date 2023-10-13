import firebaseConfig from "@/firebase/firebase-config";
import firebase from "firebase/compat/app";
import "firebase/compat/firestore";
import {
  query,
  where,
  getDocs,
  collection,
  getCountFromServer,
  getDoc,
  getDocFromServer,
  doc,
  documentId,
  updateDoc,
} from "firebase/firestore";
import { ref, onUnmounted } from "vue";

const firebaseApp = firebase.initializeApp(firebaseConfig);
export const db = firebaseApp.firestore();
export const configCollection = db.collection("config");
const runningNumberRef = configCollection.doc("running-number");
export const membersCollection = db.collection("members");
export const branchCollection = db.collection("branch");
export const sectionCollection = db.collection("section");
export const rolesCollection = db.collection("roles");
export const updateSectionBranchTrail = db.collection(
  "update_section_branch_trail"
);

const docRef = doc(db, "config", "running-number");
  const docSnap = await getDoc(docRef);

export const getFormRunningNumber = async () => {
  if (docSnap.exists()) {
    const fieldValue = docSnap.get("borang_pgrs");
    console.log('generated no siri borang : ' + fieldValue)
    return fieldValue;
  } else {
    console.error("Dokumen tidak wujud");
  }
};

export const updateFormRunningNumber = (data) => {
  updateDoc(docRef, data).then(() => {
    console.info('No siri borang berjaya di kemaskini')
  }).catch((error) => {
    console.error(error)
  })
}

export const createMember = (member) => {
  return membersCollection.add(member);
};

export const getMember = async (id) => {
  const member = await membersCollection.doc(id).get();
  return member.exists ? member.data() : null;
};

export const getRole = async (id) => {
  const role = await rolesCollection.doc(id).get();
  return role.exists ? role.data().user_role : null;
};

export const getSubRole = async (id) => {
  const subrole = await rolesCollection.doc(id).get();
  // temp fix for no subrole
  return subrole.exists ? subrole.data().user_subrole ?? "NO CAWANGAN" : null;
};

export const updateMember = (id, member) => {
  return membersCollection.doc(id).update(member);
};

export const deleteMember = (id) => {
  return membersCollection.doc(id).delete();
};

export const useLoadMember = () => {
  const members = ref([]);
  const close = membersCollection.onSnapshot((snapshot) => {
    members.value = snapshot.docs.map((doc) => ({ id: doc.id, ...doc.data() }));
  });
  onUnmounted(close);
  return members;
};

//create 21/3/2023 MASHRAN function for member data segregation
export const getDataMember = (role, subrole) => {
  const members = ref([]);
  if (role == 1) {
    const close = membersCollection.onSnapshot((snapshot) => {
      members.value = snapshot.docs.map((doc) => ({
        id: doc.id,
        ...doc.data(),
      }));
    });
  } else if (role == 2) {
    const close = membersCollection
      .where("bahagian", "==", subrole)
      .get()
      .then((snapshot) => {
        members.value = snapshot.docs.map((doc) => ({
          id: doc.id,
          ...doc.data(),
        }));
      });
  } else {
    const close = membersCollection
      .where("cawangan", "==", subrole)
      .get()
      .then((snapshot) => {
        members.value = snapshot.docs.map((doc) => ({
          id: doc.id,
          ...doc.data(),
        }));
      });
  }
  return members;
};
export const getDataMemberOnlyWithStatus = (role, subrole, status) => {
  const members = ref([]);
  if (role == 1) {
    const close = membersCollection
      .where("status", "==", status).orderBy("no_ahli", 'asc')
      .get()
      .then((snapshot) => {
        members.value = snapshot.docs.map((doc) => ({
          id: doc.id,
          ...doc.data(),
        }));
      });
  } else if (role == 2) {
    const close = membersCollection
      .where("bahagian", "==", subrole)
      .where("status", "==", status)
      .get()
      .then((snapshot) => {
        members.value = snapshot.docs.map((doc) => ({
          id: doc.id,
          ...doc.data(),
        }));
      });
  } else {
    const close = membersCollection
      .where("cawangan", "==", subrole)
      .where("status", "==", status)
      .get()
      .then((snapshot) => {
        members.value = snapshot.docs.map((doc) => ({
          id: doc.id,
          ...doc.data(),
        }));
      });
  }
  return members;
};

//create 21/3/2023 MASHRAN function for data filter by where clause
export const filterDataMember = (tableName, strField, strWhere) => {
  const data = ref([]);
  const close = db
    .collection(tableName)
    .where(strField, "==", strWhere)
    .get()
    .then((snapshot) => {
      data.value = snapshot.docs.map((doc) => ({ id: doc.id, ...doc.data() }));
    });
  onUnmounted(close);
  return data;
};
export const filterDataMemberStatus = (
  tableName,
  strField,
  strWhere,
  strStatus
) => {
  const data = ref([]);
  const close = db
    .collection(tableName)
    .where(strField, "==", strWhere)
    .where("status", "==", strStatus)
    .get()
    .then((snapshot) => {
      data.value = snapshot.docs.map((doc) => ({ id: doc.id, ...doc.data() }));
    });
  onUnmounted(close);
  return data;
};
export const filterDataMemberForStatus = (strField, strWhere, status) => {
  const data = ref([]);
  const close = membersCollection
    .where(strField, "==", strWhere)
    .where("status", "==", status)
    .get()
    .then((snapshot) => {
      data.value = snapshot.docs.map((doc) => ({ id: doc.id, ...doc.data() }));
    });
  onUnmounted(close);
  return data;
};

//create 27/3/2023 MASHRAN function for data filter by where two clause
export const filterDataMemberWithTwoClause = (
  tableName,
  strField,
  strWhere,
  strSecondField,
  strSecondWhere
) => {
  const dataRes = ref([]);
  const close = db
    .collection(tableName)
    .where(strField, "==", strWhere)
    .where(strSecondWhere, "==", strSecondField)
    .get()
    .then((snapshot) => {
      dataRes.value = snapshot.docs.map((doc) => ({
        id: doc.id,
        ...doc.data(),
      }));
    });

  onUnmounted(close);
  return dataRes;
};

// CRUD cawangan/branch
export const createBranch = (branch) => {
  return branchCollection.add(branch);
};

export const getBranch = async (id) => {
  const branch = await branchCollection.doc(id).get();
  return branch.exists ? branch.data() : null;
};

export const updateBranch = (id, branch) => {
  return branchCollection.doc(id).update(branch);
};

export const deleteBranch = (id) => {
  return branchCollection.doc(id).delete();
};

export const useLoadBranch = () => {
  const branch = ref([]);
  const close = branchCollection.onSnapshot((snapshot) => {
    branch.value = snapshot.docs.map((doc) => ({ id: doc.id, ...doc.data() }));
  });
  onUnmounted(close);
  return branch;
};

export const getDataBranch = (cawangan) => {
  const branchField = ref([]);
  if (cawangan == 1 || cawangan == "NO CAWANGAN" || cawangan == "PEMUDA" || cawangan == "WANITA" || cawangan == "BELIAWANIS") {
    const close = branchCollection.onSnapshot((snapshot) => {
      branchField.value = snapshot.docs.map((doc) => ({
        id: doc.id,
        ...doc.data(),
      }));
    });
  } else {
    const close = branchCollection
      .where("nama_bahagian", "==", cawangan)
      .get()
      .then((snapshot) => {
        branchField.value = snapshot.docs.map((doc) => ({
          id: doc.id,
          ...doc.data(),
        }));
      });
  }
  return branchField;
};

export const getFieldSectionFromBranch = (cawangan) => {
  const sectionField = ref([]);
  db.collection("branch")
    .where("nama_cawangan", "==", cawangan)
    .get()
    .then((snapshot) => {
      sectionField.value = snapshot.docs.map((doc) => ({
        id: doc.id,
        ...doc.data(),
      }));
    });
  return sectionField;
};

// export const getFieldSectionFromBranch = async (cawangan) => {
//   const q = query(collection(db, "branch"), where("nama_cawangan", "==", cawangan));
//   const resData = await getDocs(q);
//   return resData.docs.map((doc) => ({ nama_bahagian: doc.data().nama_bahagian }));
// };

export const getSectionId = async (bahagian) => {
  const q = query(
    collection(db, "section"),
    where("nama_bahagian", "==", bahagian)
  );
  const resData = await getDocs(q);
  return resData.docs.map((doc) => ({ id: doc.id, ...doc.data() }));
};

export const getBranchId = async (cawangan) => {
  const q = query(
    collection(db, "branch"),
    where("nama_cawangan", "==", cawangan)
  );
  const resData = await getDocs(q);
  return resData.docs.map((doc) => ({ id: doc.id }));
};

// 14/6/2023 MASHRAN Create function for selected item branch change for drop down list by refering to section name
export const getFieldBranchFromSection = (bahagian) => {
  const branchField = ref([]);
  db.collection("branch")
    .where("nama_bahagian", "==", bahagian)
    .get()
    .then((snapshot) => {
      branchField.value = snapshot.docs.map((doc) => ({
        id: doc.id,
        ...doc.data(),
      }));
    });
  return branchField;
};

// 17/3/2023 MASHRAN Create function for selected item change for drop down list by refering to section name
export const getFieldBranch = (cawangan) => {
  const branchField = ref([]);
  db.collection("branch")
    .where("nama_cawangan", "==", cawangan)
    .get()
    .then((snapshot) => {
      branchField.value = snapshot.docs.map((doc) => ({
        id: doc.id,
        ...doc.data(),
      }));
    });
  return branchField;
};

export const getFieldSection = (bahagian) => {
  const sectionField = ref([]);
  db.collection("section")
    .where("nama_bahagian", "==", bahagian)
    .get()
    .then((snapshot) => {
      sectionField.value = snapshot.docs.map((doc) => ({
        id: doc.id,
        ...doc.data(),
      }));
    });
  return sectionField;
};

//function for search icno
export const searchData = (icno) => {
  const searchResult = ref([]);
  const close = db
    .collection("members")
    .where("icno", "==", icno)
    .get()
    .then((snapshot) => {
      searchResult.value = snapshot.docs.map((doc) => ({
        id: doc.id,
        ...doc.data(),
      }));
    });
  return searchResult;
};

// Member Activation

export const useLoadInActiveMember = async () => {
  const q = query(
    collection(db, "members"),
    where("no_ahli", "==", "DISAHKAN")
  );
  const members = await getDocs(q);
  return members.docs.map((doc) => ({ id: doc.id, ...doc.data() }));
};
export const useLoadActiveMember = async () => {
  const q = query(collection(db, "members"), where("status", "==", "AKTIF"));
  const members = await getDocs(q);
  return members.docs.map((doc) => ({ id: doc.id, ...doc.data() }));
};

export const useLoadConfirmation = async () => {
  const q = query(collection(db, "members"), where("status", "==", "DISAHKAN"));
  const members = await getDocs(q);
  return members.docs.map((doc) => ({ id: doc.id, ...doc.data() }));
};


export const getCurrentRunningNumber = async () => {
  const doc = await runningNumberRef.get();
  if (doc.exists) {
    return doc.data().current;
  } else {
    // If the document doesn't exist create it with initial value.
    runningNumberRef.set({ current: 1 });
    return 1;
  }
};

export const updateCurrentRunningNumber = () => {
  // increment current running number
  return runningNumberRef.update({
    current: firebase.firestore.FieldValue.increment(1),
    updated_at: firebase.firestore.Timestamp.now(),
  });
};

export const updateMemberRegistrationNumber = (id, registrationNumber) => {
  return membersCollection.doc(id).update({
    no_ahli: registrationNumber,
    status: "AKTIF",
    tarikh_aktif: firebase.firestore.Timestamp.now(),
  });
};

// 28/3/2023 MASHRAN Create function for update member status sahkan/kiv/reject
export const updateMemberStatus = (
  id,
  status,
  tarikh_aktif,
  no_ahli,
  comment
) => {
  return membersCollection.doc(id).update({
    status: status,
    no_ahli: no_ahli,
    tarikh_aktif: tarikh_aktif,
    comment: comment,
  });
};

// Member Status
export const useLoadConfirmMember = async () => {
  const q = query(
    collection(db, "members"),
    where("no_ahli", "==", "DISAHKAN")
  );
  const members = await getDocs(q);
  return members.docs.map((doc) => ({ id: doc.id, ...doc.data() }));
};

// CRUD bahagian/section
export const createSection = (section) => {
  return sectionCollection.add(section);
};

export const getSection = async (id) => {
  const section = await sectionCollection.doc(id).get();
  return section.exists ? section.data() : null;
};

export const updateSection = (id, section) => {
  return sectionCollection.doc(id).update(section);
};

export const deleteSection = (id) => {
  return sectionCollection.doc(id).delete();
};

export const useLoadSection = () => {
  const section = ref([]);
  const close = sectionCollection.onSnapshot((snapshot) => {
    section.value = snapshot.docs.map((doc) => ({ id: doc.id, ...doc.data() }));
  });
  onUnmounted(close);
  return section;
};

export const getDataSection = (bahagian) => {
  const sectionField = ref([]);
  if (bahagian == 1 || bahagian == "NO CAWANGAN" || bahagian == "PEMUDA" || bahagian == "WANITA" || bahagian == "BELIAWANIS") {
    const close = sectionCollection.onSnapshot((snapshot) => {
      sectionField.value = snapshot.docs.map((doc) => ({
        id: doc.id,
        ...doc.data(),
      }));
    });
  } else {
    const close = db
      .collection("section")
      .where("nama_bahagian", "==", bahagian)
      .get()
      .then((snapshot) => {
        sectionField.value = snapshot.docs.map((doc) => ({
          id: doc.id,
          ...doc.data(),
        }));
      });
  }
  return sectionField;
};

export const getDataSectionWithRole = (role, fieldValue) => {
  const sectionField = ref([]);
  if (role == 1) {
    sectionCollection.onSnapshot((snapshot) => {
      sectionField.value = snapshot.docs.map((doc) => ({
        id: doc.id,
        ...doc.data(),
      }));
    });
  } else if (role == 2) {
    db.collection("section")
      .where("nama_bahagian", "==", fieldValue)
      .get()
      .then((snapshot) => {
        sectionField.value = snapshot.docs.map((doc) => ({
          id: doc.id,
          ...doc.data(),
        }));
      });
  } else if (role == 3) {
    db.collection("branch")
      .where("nama_cawangan", "==", fieldValue)
      .get()
      .then((snapshot) => {
        sectionField.value = snapshot.docs.map((doc) => ({
          id: doc.id,
          ...doc.data(),
        }));
      });
  }
  return sectionField;
};

export const getDataBranchWithRole = (role, fieldValue) => {
  const sectionField = ref([]);
  if (role == 1) {
    sectionCollection.onSnapshot((snapshot) => {
      sectionField.value = snapshot.docs.map((doc) => ({
        id: doc.id,
        ...doc.data(),
      }));
    });
  } else if (role == 2) {
    db.collection("branch")
      .where("nama_bahagian", "==", fieldValue)
      .get()
      .then((snapshot) => {
        sectionField.value = snapshot.docs.map((doc) => ({
          id: doc.id,
          ...doc.data(),
        }));
      });
  } else if (role == 3) {
    db.collection("branch")
      .where("nama_cawangan", "==", fieldValue)
      .get()
      .then((snapshot) => {
        sectionField.value = snapshot.docs.map((doc) => ({
          id: doc.id,
          ...doc.data(),
        }));
      });
  }
  return sectionField;
};

export const getListMember = (cawangan) => {
  const membersField = ref([]);
  if (cawangan == 1 || cawangan == "NO CAWANGAN") {
    const close = membersCollection.onSnapshot((snapshot) => {
      sectionField.value = snapshot.docs.map((doc) => ({
        id: doc.id,
        ...doc.data(),
      }));
    });
  } else {
    const close = db
      .collection("members")
      .where("cawangan", "==", cawangan)
      .get()
      .then((snapshot) => {
        membersField.value = snapshot.docs.map((doc) => ({
          id: doc.id,
          ...doc.data(),
        }));
      });
  }
  return membersField;
};

//function for dashboard
export const countData = async (tblName) => {
  const res = ref([]);
  const coll = collection(db, tblName);
  const close = await getCountFromServer(coll).then((snapshot) => {
    res.value = snapshot
      .data()
      .count.toString()
      .replace(/\B(?=(\d{3})+(?!\d))/g, ",");
  });
  return res;
};

// 30/3/2023 MASHRAN Create function for count document by collection with query
export const countDataWithQuery = async (
  tblName,
  strField,
  strOperator,
  strWhere
) => {
  const coll = collection(db, tblName);
  const queryColl = query(coll, where(strField, strOperator, strWhere));
  const close = await getCountFromServer(queryColl).then((snapshot) => {
    return snapshot
      .data()
      .count.toString()
      .replace(/\B(?=(\d{3})+(?!\d))/g, ",");
  });
  return close;
};

export const countDataWithTwoQuery = async (
  tblName,
  strField,
  strOperator,
  strWhere,
  strSecondField,
  strSecondOperator,
  strSecondWhere
) => {
  const res = ref([]);
  const coll = collection(db, tblName);
  const queryColl = query(
    coll,
    where(strField, strOperator, strWhere),
    where(strSecondField, strSecondOperator, strSecondWhere)
  );
  const close = await getCountFromServer(queryColl).then((snapshot) => {
    return snapshot
      .data()
      .count.toString()
      .replace(/\B(?=(\d{3})+(?!\d))/g, ",");
  });
  return close;
};

// 30/3/2023 MASHRAN Create function for count document by collection with query
// export const countDataWithTwoQuery = async (
//   tblName,
//   strField,
//   strOperator,
//   strWhere,
//   strSecondField,
//   strSecondOperator,
//   strSecondWhere
// ) => {
//   //exp operator = array-contains-any
//   const coll = collection(db, tblName);
//   const queryColl = query(
//     coll,
//     where(strField, strOperator, strWhere),
//     where(strSecondField, strSecondOperator, strSecondWhere)
//   );
//   const snapshot = await getCountFromServer(queryColl);
//   const totalCount = snapshot.data().count;
//   return totalCount;
// };

export const getDataWithTwoQuery = async (
  tblName,
  strField,
  strOperator,
  strWhere,
  strSecondField,
  strSecondOperator,
  strSecondWhere
) => {
  //exp operator = array-contains-any
  const coll = collection(db, tblName);
  const queryColl = query(
    coll,
    where(strField, strOperator, strWhere),
    where(strSecondField, strSecondOperator, strSecondWhere)
  );
  const snapshot = await getDocs(queryColl);

  return snapshot.docs.map((doc) => ({ id: doc.id, ...doc.data() }));
};

//create 5/6/2023 MASHRAN function for get data record from database with query segregation
export const getDataTwoWhere = (
  role,
  tableName,
  fieldName,
  strOperator,
  whereValue,
  secondFieldName,
  secondStrOperator,
  secondWhereValue
) => {
  const coll = db.collection(tableName);
  const dataRes = ref([]);
  if (role == 1) {
    const close = coll
      .where(fieldName, strOperator, whereValue)
      .where(secondFieldName, secondStrOperator, secondWhereValue)
      .get()
      .then((snapshot) => {
        dataRes.value = snapshot.docs.map((doc) => ({
          id: doc.id,
          ...doc.data(),
        }));
      });
  } else if (role == 2) {
    const close = coll
      .where(fieldName, strOperator, whereValue)
      .where(secondFieldName, secondStrOperator, secondWhereValue)
      .get()
      .then((snapshot) => {
        dataRes.value = snapshot.docs.map((doc) => ({
          id: doc.id,
          ...doc.data(),
        }));
      });
  } else {
    const close = coll
      .where(fieldName, strOperator, whereValue)
      .where(secondFieldName, secondStrOperator, secondWhereValue)
      .get()
      .then((snapshot) => {
        dataRes.value = snapshot.docs.map((doc) => ({
          id: doc.id,
          ...doc.data(),
        }));
      });
  }
  return dataRes;
};

export const getData = (tableName, fieldName, strOperator, whereValue) => {
  const coll = db.collection(tableName);
  const dataRes = ref([]);
  const close = coll
    .where(fieldName, strOperator, whereValue)
    .get()
    .then((snapshot) => {
      dataRes.value = snapshot.docs.map((doc) => ({
        id: doc.id,
        ...doc.data(),
      }));
    });
  return dataRes;
};

//GET SECTION ID
export const getCurrentID = async (typeofid) => {
  const runningID = await configCollection.doc(typeofid).get();
  return runningID.exists ? runningID.data().current : null;
};

export const updateCurrentID = (typeofid) => {
  // increment current running number
  return configCollection.doc(typeofid).update({
    current: firebase.firestore.FieldValue.increment(1),
    updated_at: firebase.firestore.Timestamp.now(),
  });
};

//17-6-2023 : MASHRAN CREATE COUNT BY GROUP FOR EACH BRANCH AND MEMBERS REFER TO SECTION NAME
export const getDataSectionWithCountByGroup = () => {
  const resData = ref([]);
  db.collection("section").onSnapshot((snapshot) => {
    resData.value = snapshot.docs.map((doc) => ({
      id: doc.id,
      ...doc.data(),
    }));
  });
  return resData;
};

export const getDataBranchWithCountByGroup = (role) => {
  const resData = ref([]);
  if (role == 1) {
    db.collection("branch").onSnapshot((snapshot) => {
      resData.value = snapshot.docs.map((doc) => ({
        id: doc.id,
        ...doc.data(),
      }));
    });
  } else {
    db.collection("branch")
      .where("nama_bahagian", "==", role)
      .get()
      .then((snapshot) => {
        resData.value = snapshot.docs.map((doc) => ({
          id: doc.id,
          ...doc.data(),
        }));
      });
  }
  return resData;
};

export const countBranchBySection = (bahagian) => {
  const totalBranch = ref([]);
  const close = db
    .collection("branch")
    .where("nama_bahagian", "==", bahagian)
    .get()
    .then((snapshot) => {
      totalBranch.value = snapshot.docs.length;
    });
  return totalBranch;
};

export const countMembersBySection = (bahagian) => {
  const totalMembers = ref([]);
  const close = db
    .collection("members")
    .where("bahagian", "==", bahagian)
    .get()
    .then((snapshot) => {
      totalMembers.value = snapshot.docs.length;
    });
  return totalMembers;
};

export const countMembersByBranch = (cawangan) => {
  const totalMembers = ref([]);
  const close = db
    .collection("members")
    .where("cawangan", "==", cawangan)
    .get()
    .then((snapshot) => {
      totalMembers.value = snapshot.docs.length;
    });
  return totalMembers;
};

export const getDataByQuery = (
  _collectionName,
  _orderByField,
  _startAfter,
  _endBefore,
  _rowLimit,
  _whereClause,
  _strOperator,
  _whereValue,
  _direction
) => {
  const coll = db.collection(_collectionName);
  if (
    _startAfter == null &&
    _endBefore == null &&
    _rowLimit == null &&
    _whereClause == null &&
    _strOperator == null &&
    _whereValue == null
  ) {
    return coll.orderBy(_orderByField, _direction).get();
  } else if (
    _startAfter == null &&
    _endBefore == null &&
    _whereClause == null &&
    _strOperator == null &&
    _whereValue == null
  ) {
    return coll.orderBy(_orderByField, _direction).limit(_rowLimit).get();
  } else if (
    _endBefore == null &&
    _whereClause == null &&
    _strOperator == null &&
    _whereValue == null
  ) {
    return coll
      .orderBy(_orderByField, _direction)
      .startAfter(_startAfter)
      .limit(_rowLimit)
      .get();
  } else if (
    _startAfter == null &&
    _whereClause == null &&
    _strOperator == null &&
    _whereValue == null
  ) {
    return coll
      .orderBy(_orderByField, _direction)
      .endBefore(_endBefore)
      .limitToLast(_rowLimit)
      .get();
  } else if (_startAfter == null && _endBefore == null) {
    return coll
      .where(_whereClause, _strOperator, _whereValue)
      .orderBy(_orderByField, _direction)
      .limit(_rowLimit)
      .get();
  } else if (_startAfter == null && _endBefore == null && _rowLimit == null) {
    return coll
      .where(_whereClause, _strOperator, _whereValue)
      .orderBy(_orderByField, _direction)
      .get();
  } else if (_startAfter == null) {
    return coll
      .where(_whereClause, _strOperator, _whereValue)
      .orderBy(_orderByField, _direction)
      .endBefore(_endBefore)
      .limitToLast(_rowLimit)
      .get();
  } else if (_endBefore == null) {
    return coll
      .where(_whereClause, _strOperator, _whereValue)
      .orderBy(_orderByField, _direction)
      .startAfter(_startAfter)
      .limit(_rowLimit)
      .get();
  }
};

export const getDataByQueryTwoWhere = (
  _collectionName,
  _orderByField,
  _startAfter,
  _endBefore,
  _rowLimit,
  _whereClause,
  _strOperator,
  _whereValue,
  _secondWhereClause,
  _strSecondOperator,
  _secondWhereValue,
  _orderDirection
) => {
  const coll = db.collection(_collectionName);
  if (
    !_startAfter &&
    !_endBefore &&
    !_rowLimit &&
    !_secondWhereClause &&
    !_strSecondOperator &&
    !_secondWhereValue
  ) {
    return coll
      .where(_whereClause, _strOperator, _whereValue)
      .orderBy(_orderByField, _orderDirection)
      .get();
  } else if (
    !_startAfter &&
    !_endBefore &&
    !_secondWhereClause &&
    !_strSecondOperator &&
    !_secondWhereValue
  ) {
    return coll
      .where(_whereClause, _strOperator, _whereValue)
      .orderBy(_orderByField, _orderDirection)
      .limit(_rowLimit)
      .get();
  } else if (
    _endBefore == null &&
    _secondWhereClause == null &&
    _strSecondOperator == null &&
    _secondWhereValue == null
  ) {
    return coll
      .where(_whereClause, _strOperator, _whereValue)
      .orderBy(_orderByField, _orderDirection)
      .startAfter(_startAfter)
      .limit(_rowLimit)
      .get();
  } else if (
    _startAfter == null &&
    _secondWhereClause == null &&
    _strSecondOperator == null &&
    _secondWhereValue == null
  ) {
    return coll
      .where(_whereClause, _strOperator, _whereValue)
      .orderBy(_orderByField, _orderDirection)
      .endBefore(_endBefore)
      .limitToLast(_rowLimit)
      .get();
  } else if (_startAfter == null && _endBefore == null) {
    return coll
      .where(_whereClause, _strOperator, _whereValue)
      .where(_secondWhereClause, _strSecondOperator, _secondWhereValue)
      .orderBy(_orderByField, _orderDirection)
      .limit(_rowLimit)
      .get();
  } else if (_startAfter == null && _endBefore == null && _rowLimit == null) {
    return coll
      .where(_whereClause, _strOperator, _whereValue)
      .where(_secondWhereClause, _strSecondOperator, _secondWhereValue)
      .orderBy(_orderByField, _orderDirection)
      .get();
  } else if (_startAfter == null) {
    return coll
      .where(_whereClause, _strOperator, _whereValue)
      .where(_secondWhereClause, _strSecondOperator, _secondWhereValue)
      .orderBy(_orderByField, _orderDirection)
      .endBefore(_endBefore)
      .limitToLast(_rowLimit)
      .get();
  } else if (_endBefore == null) {
    return coll
      .where(_whereClause, _strOperator, _whereValue)
      .where(_secondWhereClause, _strSecondOperator, _secondWhereValue)
      .orderBy(_orderByField, _orderDirection)
      .startAfter(_startAfter)
      .limit(_rowLimit)
      .get();
  }
};

export async function updateMultipleDocument(
  _collectionName,
  _strOperator,
  _docID,
  _newData
) {
  try {
    const collectionRef = db.collection(_collectionName);
    const batches = [];
    while (_docID.length) {
      const batch = _docID.splice(0, 10);
      batches.push(
        await collectionRef
          .where(firebase.firestore.FieldPath.documentId(), _strOperator, [
            ...batch,
          ])
          .get()
          .then((querySnapshot) =>
            querySnapshot.forEach(async (doc) => {
              const docRef = collectionRef.doc(doc.id);
              await docRef.update(_newData);
            })
          )
      );
    }
    return 'success';
  } catch (error) {
    console.error("Error updating data:", error);
    return "failed";
  }
}

export const searchDataByQuery = (
  _role,
  _subrole,
  _collectionName,
  _whereClause,
  _strOperator,
  _whereValue,
  _secondWhereClause,
  _strSecondOperator,
  _secondWhereValue
) => {
  const coll = db.collection(_collectionName);
  if (_role == 1) {
    return coll
      .where(_whereClause, _strOperator, _whereValue)
      .where(_secondWhereClause, _strSecondOperator, _secondWhereValue)
      .get();
  } else if (_role == 2) {
    return coll
      .where("bahagian", "==", _subrole)
      .where(_whereClause, _strOperator, _whereValue)
      .where(_secondWhereClause, _strSecondOperator, _secondWhereValue)
      .get();
  } else if (_role == 3) {
    return coll
      .where("cawangan", "==", _subrole)
      .where(_whereClause, _strOperator, _whereValue)
      .where(_secondWhereClause, _strSecondOperator, _secondWhereValue)
      .get();
  }
};
export async function updateExchangeStatus(
  id,
  status,
  head_of_section,
  general_secretary,
  updated_at
) {
  try {
    await updateSectionBranchTrail.doc(id).update({
      status: status,
      head_of_section: head_of_section,
      general_secretary: general_secretary,
      updated_at: updated_at,
    });
    return "success";
  } catch (error) {
    return "failed";
  }
}
export async function updateExchangeOnMembers(
  id,
  branch_name,
  section_name,
  updated_at
) {
  try {
    await membersCollection.doc(id).update({
      cawangan: branch_name,
      bahagian: section_name,
      updated_at: updated_at,
    });
    return "success";
  } catch (error) {
    return "failed";
  }
}

export async function updateCountSection(nama_bahagian) {
  const countMembers = await countDataWithQuery(
    "members",
    "bahagian",
    "==",
    nama_bahagian
  );
  const countAhliBiasa = await countDataWithTwoQuery(
    "members",
    "bahagian",
    "==",
    nama_bahagian,
    "keahlian",
    "==",
    "Ahli Biasa"
  );
  const countPemuda = await countDataWithTwoQuery(
    "members",
    "bahagian",
    "==",
    nama_bahagian,
    "keahlian",
    "==",
    "Pemuda"
  );
  const countWanita = await countDataWithTwoQuery(
    "members",
    "bahagian",
    "==",
    nama_bahagian,
    "keahlian",
    "==",
    "Wanita"
  );
  const countBeliawanis = await countDataWithTwoQuery(
    "members",
    "bahagian",
    "==",
    nama_bahagian,
    "keahlian",
    "==",
    "Beliawanis"
  );
  const sectionData = {
    jumlah_ahli: countMembers,
    jumlah_ahli_biasa: countAhliBiasa,
    jumlah_wanita: countWanita,
    jumlah_pemuda: countPemuda,
    jumlah_beliawanis: countBeliawanis,
  };
  const sectionID = await getSectionId(nama_bahagian);
  updateSection(sectionID[0].id, sectionData);
}
export async function updateCountBranch(nama_cawangan) {
  const countMembers = await countDataWithQuery(
    "members",
    "cawangan",
    "==",
    nama_cawangan
  );
  const countAhliBiasa = await countDataWithTwoQuery(
    "members",
    "cawangan",
    "==",
    nama_cawangan,
    "keahlian",
    "==",
    "Ahli Biasa"
  );
  const countPemuda = await countDataWithTwoQuery(
    "members",
    "cawangan",
    "==",
    nama_cawangan,
    "keahlian",
    "==",
    "Pemuda"
  );
  const countWanita = await countDataWithTwoQuery(
    "members",
    "cawangan",
    "==",
    nama_cawangan,
    "keahlian",
    "==",
    "Wanita"
  );
  const countBeliawanis = await countDataWithTwoQuery(
    "members",
    "cawangan",
    "==",
    nama_cawangan,
    "keahlian",
    "==",
    "Beliawanis"
  );
  const branchData = {
    jumlah_ahli: countMembers,
    jumlah_ahli_biasa: countAhliBiasa,
    jumlah_wanita: countWanita,
    jumlah_pemuda: countPemuda,
    jumlah_beliawanis: countBeliawanis,
  };
  const branchID = await getBranchId(nama_cawangan);
  updateBranch(branchID[0].id, branchData);
}