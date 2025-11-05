import './App.css';
import React, { useState, useEffect } from 'react';

// --- IMPORTANT ---
// Make sure this port number matches your C# API's port!
const API_URL = 'https://localhost:7268'; // Use your port
// -----------------

function App() {
  const [groups, setGroups] = useState([]);
  const [formData, setFormData] = useState({ id: 0, courseName: '', groupName: '' });
  const [editingId, setEditingId] = useState(null);

  // --- Load initial data ---
  useEffect(() => {
    fetchGroups();
  }, []);

  // --- GET Function (Read) ---
  const fetchGroups = async () => {
    try {
      // This endpoint is now our "smart" endpoint!
      const response = await fetch(`${API_URL}/api/groups`);
      const data = await response.json();
      setGroups(data);
    } catch (error) {
      console.error("Error fetching groups:", error);
    }
  };

  // --- Form Change & Reset Functions (no change) ---
  const handleFormChange = (event) => {
    const { name, value } = event.target;
    setFormData(prevData => ({ ...prevData, [name]: value }));
  };

  const resetForm = () => {
    setEditingId(null);
    setFormData({ id: 0, courseName: '', groupName: '' });
  };

  // --- Form Submit (Create/Update) Function (no change) ---
  const handleFormSubmit = async (event) => {
    event.preventDefault();
    const url = editingId ? `${API_URL}/api/groups/${editingId}` : `${API_URL}/api/groups`;
    const method = editingId ? 'PUT' : 'POST';
    
    try {
      const response = await fetch(url, {
        method: method,
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ ...formData, id: editingId || 0 })
      });

      if (response.ok) {
        console.log(`Group ${editingId ? 'updated' : 'created'}!`);
        resetForm();
        fetchGroups(); // Refresh the list
      } else {
        console.error("Form submit failed.");
      }
    } catch (error) {
      console.error("Error submitting form:", error);
    }
  };

  // --- Edit/Delete Group Functions (no change) ---
  const handleEditClick = (group) => {
    setEditingId(group.id);
    setFormData(group);
  };

  const handleDeleteGroup = async (idToDelete) => {
    if (!window.confirm("Are you sure you want to DELETE this group?")) return;
    try {
      const response = await fetch(`${API_URL}/api/groups/${idToDelete}`, {
        method: 'DELETE'
      });
      if (response.ok) {
        fetchGroups(); // Refresh list
      } else {
        console.error("Failed to delete group.");
      }
    } catch (error) {
      console.error("Error deleting group:", error);
    }
  };

  // --- NEW: "JOIN" FUNCTION ---
  const handleJoin = async (groupId) => {
    try {
      const response = await fetch(`${API_URL}/api/groups/${groupId}/join`, {
        method: 'POST'
      });
      if (response.ok) {
        console.log("Joined group!");
        fetchGroups(); // Refresh the list to get new membership status
      } else {
        console.error("Failed to join group.");
      }
    } catch (error) {
      console.error("Error joining group:", error);
    }
  };

  // --- NEW: "LEAVE" FUNCTION ---
  const handleLeave = async (groupId) => {
    if (!window.confirm("Are you sure you want to LEAVE this group?")) return;
    try {
      const response = await fetch(`${API_URL}/api/groups/${groupId}/leave`, {
        method: 'DELETE'
      });
      if (response.ok) {
        console.log("Left group!");
        fetchGroups(); // Refresh the list to get new membership status
      } else {
        console.error("Failed to leave group.");
      }
    } catch (error) {
      console.error("Error leaving group:", error);
    }
  };


  // --- UPDATED: The HTML (JSX) ---
  return (
    <div className="App">
      <header className="App-header">
        <h1>Welcome to StudySphere</h1>

        {/* --- "Smart" Form (no change) --- */}
        <form onSubmit={handleFormSubmit} style={styles.form}>
          <h3 style={{ color: '#333', marginTop: 0 }}>
            {editingId ? "Edit Group" : "Create a New Group"}
          </h3>
          <div>
            <input 
              type="text" 
              name="courseName"
              placeholder="Course Name"
              value={formData.courseName}
              onChange={handleFormChange}
              required
              style={{ ...styles.input, marginRight: '10px' }}
            />
            <input 
              type="text" 
              name="groupName"
              placeholder="Group Name"
              value={formData.groupName}
              onChange={handleFormChange}
              required
              style={styles.input}
            />
          </div>
          <button type="submit" style={styles.button}>
            {editingId ? "Update Group" : "Create Group"}
          </button>
          {editingId && (
            <button type="button" onClick={resetForm} style={styles.cancelButton}>
              Cancel Edit
            </button>
          )}
        </form>

        {/* --- The List of Groups (NOW WITH ALL BUTTONS) --- */}
        <h2>Available Groups (Live from API!)</h2>
        {groups.map(group => (
          <div key={group.id} style={styles.card}>
            {/* Group Info */}
            <div>
              <h3>{group.groupName}</h3>
              {/* Note: property names are camelCased by default JSON serialization */}
              <p>Course: {group.courseName} (ID: {group.id})</p>
            </div>
            
            {/* Button Container */}
            <div style={{ display: 'flex', gap: '10px' }}>
              
              {/* --- NEW: CONDITIONAL "JOIN/LEAVE" BUTTON --- */}
              {/* This is the magic! It reads our new property */}
              {group.isDemoUserMember ? (
                <button 
                  onClick={() => handleLeave(group.id)} 
                  style={{...styles.button, ...styles.leaveButton}}
                >
                  Leave
                </button>
              ) : (
                <button 
                  onClick={() => handleJoin(group.id)} 
                  style={{...styles.button, ...styles.joinButton}}
                >
                  Join
                </button>
              )}

              {/* --- Admin Buttons (Edit/Delete Group) --- */}
              <button 
                onClick={() => handleEditClick(group)} 
                style={{...styles.button, ...styles.editButton}}
              >
                Edit
              </button>
              <button 
                onClick={() => handleDeleteGroup(group.id)} 
                style={{...styles.button, ...styles.deleteButton}}
              >
                Delete
              </button>
            </div>
          </div>
        ))}
      </header>
    </div>
  );
}

// --- NEW: A simple 'styles' object to clean up the JSX ---
// This is a common pattern in React to make the HTML part easier to read.
const styles = {
  form: { 
    background: '#f0f0f0', 
    padding: '20px', 
    borderRadius: '8px',
    marginBottom: '30px',
    width: '500px'
  },
  input: {
    padding: '8px', 
    width: '200px'
  },
  card: { 
    border: '1px solid #ccc', 
    borderRadius: '8px', 
    margin: '16px', 
    padding: '16px',
    width: '600px', // Wider to fit all buttons
    display: 'flex',
    justifyContent: 'space-between',
    alignItems: 'center'
  },
  button: {
    color: 'white',
    border: 'none',
    padding: '8px 12px',
    borderRadius: '4px',
    cursor: 'pointer',
    fontSize: '14px'
  },
  cancelButton: {
    marginTop: '15px',
    marginLeft: '10px',
    padding: '10px 20px', 
    fontSize: '16px',
    cursor: 'pointer',
    background: '#aaa'
  },
  joinButton: {
    background: '#28a745' // Green
  },
  leaveButton: {
    background: '#dc3545' // Red
  },
  editButton: {
    background: '#007bff' // Blue
  },
  deleteButton: {
    background: '#6c757d' // Gray
  }
};

export default App;