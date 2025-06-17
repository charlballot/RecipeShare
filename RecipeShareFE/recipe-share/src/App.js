import React, { useState, useEffect } from 'react';
import { Search, Plus, Edit, Trash2, Save, X, Clock, Users, ChefHat } from 'lucide-react';

const RecipeManager = () => {
  const [recipes, setRecipes] = useState([]);
  const [currentView, setCurrentView] = useState('list');
  const [selectedRecipe, setSelectedRecipe] = useState(null);
  const [editingRecipe, setEditingRecipe] = useState(null);
  const [searchTerm, setSearchTerm] = useState('');
  const [errors, setErrors] = useState({});
  const [loading, setLoading] = useState(false);
  const [message, setMessage] = useState('');

  // API Base URL - adjust port as needed
  const API_BASE_URL = 'https://localhost:44364/api';

  // API Helper Functions
  const apiCall = async (endpoint, options = {}) => {
    try {
      const response = await fetch(`${API_BASE_URL}${endpoint}`, {
        headers: {
          'Content-Type': 'application/json',
          ...options.headers,
        },
        ...options,
      });

      if (!response.ok) {
        if (response.status === 400) {
          // Handle validation errors from api
          const errorData = await response.json();
          throw new ValidationError(errorData);
        }
        throw new Error(`HTTP error! status: ${response.status}`);
      }

      return await response.json();
    } catch (error) {
      console.error('API call failed:', error);
      throw error;
    }
  };

  // Custom error class for validation errors
  class ValidationError extends Error {
    constructor(errorData) {
      super('Validation failed');
      this.name = 'ValidationError';
      this.errors = errorData.errors || errorData;
    }
  }

  // Load recipes from api
  const loadRecipes = async () => {
    setLoading(true);
    try {
      const data = await apiCall('/recipes');
      setRecipes(data);
    } catch (error) {
      setMessage('Failed to load recipes. Please check if your API is running.');
      console.error('Failed to load recipes:', error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadRecipes();
  }, []);

  const handleSave = async (recipe) => {
    setErrors({});
    setLoading(true);
    
    try {
      let savedRecipe;
      
      if (recipe.id) {
        // Update existing recipe
        savedRecipe = await apiCall(`/recipes/${recipe.id}`, {
          method: 'PUT',
          body: JSON.stringify(recipe),
        });
        setRecipes(prev => prev.map(r => r.id === recipe.id ? savedRecipe : r));
        setMessage('Recipe updated successfully!');
      } else {
        // Add new recipe
        savedRecipe = await apiCall('/recipes', {
          method: 'POST',
          body: JSON.stringify(recipe),
        });
        setRecipes(prev => [...prev, savedRecipe]);
        setMessage('Recipe added successfully!');
      }
      
      setEditingRecipe(null);
      setCurrentView('list');
      
      // Clear message after 3 seconds
      setTimeout(() => setMessage(''), 3000);
      
    } catch (error) {
      if (error instanceof ValidationError) {
        // Handle api validation errors
        const formattedErrors = {};
        
        // Handle different error formats from api
        if (error.errors) {
          Object.keys(error.errors).forEach(key => {
            // Convert PascalCase property names to camelCase for frontend
            const camelCaseKey = key.charAt(0).toLowerCase() + key.slice(1);
            formattedErrors[camelCaseKey] = Array.isArray(error.errors[key]) 
              ? error.errors[key][0] 
              : error.errors[key];
          });
        }
        
        setErrors(formattedErrors);
        setMessage('Please fix the validation errors below');
      } else {
        setMessage('Failed to save recipe. Please try again.');
      }
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async (id) => {
    if (window.confirm('Are you sure you want to delete this recipe?')) {
      setLoading(true);
      try {
        await apiCall(`/recipes/${id}`, {
          method: 'DELETE',
        });
        
        setRecipes(prev => prev.filter(r => r.id !== id));
        setMessage('Recipe deleted successfully!');
        setCurrentView('list');
        setTimeout(() => setMessage(''), 3000);
        
      } catch (error) {
        setMessage('Failed to delete recipe. Please try again.');
      } finally {
        setLoading(false);
      }
    }
  };

  const filteredRecipes = recipes.filter(recipe =>
    recipe.title?.toLowerCase().includes(searchTerm.toLowerCase()) ||
    recipe.dietaryTags?.some(tag => tag.toLowerCase().includes(searchTerm.toLowerCase()))
  );

  const RecipeForm = ({ recipe, onSave, onCancel }) => {
    const [formData, setFormData] = useState(() => {
    // Ensure all required properties have proper default values
    const defaultRecipe = {
      title: '',
      ingredients: [''],
      steps: [''],
      cookingTime: '',
      dietaryTags: []
    };
    
    // If editing an existing recipe, merge with defaults to ensure all properties exist
    if (recipe && recipe.id) {
      return {
        ...defaultRecipe,
        ...recipe,
        // Ensure arrays are never undefined
        ingredients: recipe.ingredients || [''],
        steps: recipe.steps || [''],
        dietaryTags: recipe.dietaryTags || []
      };
    }
    
    // For new recipes, return the default structure
    return defaultRecipe;
  });

    const handleInputChange = (field, value) => {
      setFormData(prev => ({ ...prev, [field]: value }));
    };

    const handleArrayChange = (field, index, value) => {
      setFormData(prev => ({
        ...prev,
        [field]: prev[field].map((item, i) => i === index ? value : item)
      }));
    };

    const addArrayItem = (field) => {
      setFormData(prev => ({
        ...prev,
        [field]: [...prev[field], '']
      }));
    };

    const removeArrayItem = (field, index) => {
      setFormData(prev => ({
        ...prev,
        [field]: prev[field].filter((_, i) => i !== index)
      }));
    };

    return (
      <div className="max-w-4xl mx-auto p-6 bg-white rounded-lg shadow-lg">
        <h2 className="text-2xl font-bold mb-6 text-gray-800">
          {recipe?.id ? 'Edit Recipe' : 'Add New Recipe'}
        </h2>
        
        <div className="space-y-6">
          <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Title *
              </label>
              <input
                type="text"
                value={formData.title}
                onChange={(e) => handleInputChange('title', e.target.value)}
                className={`w-full px-3 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 ${
                  errors.title ? 'border-red-500' : 'border-gray-300'
                }`}
                placeholder="Recipe title"
              />
              {errors.title && <p className="mt-1 text-sm text-red-600">{errors.title}</p>}
            </div>
          </div>

          <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Cooking Time (minutes) *
              </label>
              <input
                type="number"
                value={formData.cookingTime}
                onChange={(e) => handleInputChange('cookingTime', parseInt(e.target.value))}
                className={`w-full px-3 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 ${
                  errors.cookingTime ? 'border-red-500' : 'border-gray-300'
                }`}
                min="1"
              />
              {errors.cookingTime && <p className="mt-1 text-sm text-red-600">{errors.cookingTime}</p>}
            </div>
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Ingredients *
            </label>
            {formData.ingredients.map((ingredient, index) => (
              <div key={index} className="flex gap-2 mb-2">
                <input
                  type="text"
                  value={ingredient}
                  onChange={(e) => handleArrayChange('ingredients', index, e.target.value)}
                  className="flex-1 px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                  placeholder="Ingredient"
                />
                <button
                  onClick={() => removeArrayItem('ingredients', index)}
                  className="px-3 py-2 text-red-600 hover:bg-red-50 rounded-md"
                  disabled={formData.ingredients.length === 1}
                >
                  <X size={16} />
                </button>
              </div>
            ))}
            <button
              onClick={() => addArrayItem('ingredients')}
              className="text-blue-600 hover:text-blue-800 text-sm"
            >
              + Add Ingredient
            </button>
            {errors.ingredients && <p className="mt-1 text-sm text-red-600">{errors.ingredients}</p>}
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Steps *
            </label>
            {formData.steps.map((step, index) => (
              <div key={index} className="flex gap-2 mb-2">
                <div className="flex-shrink-0 w-8 h-8 bg-blue-100 rounded-full flex items-center justify-center text-sm font-medium text-blue-600">
                  {index + 1}
                </div>
                <textarea
                  value={step}
                  onChange={(e) => handleArrayChange('steps', index, e.target.value)}
                  className="flex-1 px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                  placeholder="Step description"
                  rows="2"
                />
                <button
                  onClick={() => removeArrayItem('steps', index)}
                  className="px-3 py-2 text-red-600 hover:bg-red-50 rounded-md self-start"
                  disabled={formData.steps.length === 1}
                >
                  <X size={16} />
                </button>
              </div>
            ))}
            <button
              onClick={() => addArrayItem('steps')}
              className="text-blue-600 hover:text-blue-800 text-sm"
            >
              + Add Step
            </button>
            {errors.steps && <p className="mt-1 text-sm text-red-600">{errors.steps}</p>}
          </div>

          <div className="flex gap-4 pt-4 border-t">
            <button
              onClick={() => onSave(formData)}
              disabled={loading}
              className="flex items-center gap-2 px-6 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 disabled:opacity-50"
            >
              <Save size={16} />
              {loading ? 'Saving...' : 'Save Recipe'}
            </button>
            <button
              onClick={onCancel}
              className="flex items-center gap-2 px-6 py-2 border border-gray-300 rounded-md hover:bg-gray-50"
            >
              <X size={16} />
              Cancel
            </button>
          </div>
        </div>
      </div>
    );
  };

  const RecipeList = () => (
    <div className="max-w-6xl mx-auto p-6">
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-3xl font-bold text-gray-800">Recipe Manager</h1>
        <button
          onClick={() => {
            setEditingRecipe({});
            setCurrentView('form');
            setErrors({});
          }}
          className="flex items-center gap-2 px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700"
        >
          <Plus size={16} />
          Add Recipe
        </button>
      </div>

      <div className="mb-6">
        <div className="relative">
          <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400" size={20} />
          <input
            type="text"
            placeholder="Search recipes..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
          />
        </div>
      </div>

      {loading ? (
        <div className="text-center py-8">
          <div className="inline-block animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600"></div>
          <p className="mt-2 text-gray-600">Loading recipes...</p>
        </div>
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          {filteredRecipes.map(recipe => (
            <div key={recipe.id} className="bg-white rounded-lg shadow-md hover:shadow-lg transition-shadow">
              <div className="p-6">
                <div className="flex justify-between items-start mb-3">
                  <h3 className="text-xl font-semibold text-gray-800 truncate">{recipe.title}</h3>
                  <div className="flex gap-2 ml-2">
                    <button
                      onClick={() => {
                        setEditingRecipe(recipe);
                        setCurrentView('form');
                        setErrors({});
                      }}
                      className="p-1 text-blue-600 hover:bg-blue-50 rounded"
                    >
                      <Edit size={16} />
                    </button>
                    <button
                      onClick={() => handleDelete(recipe.id)}
                      className="p-1 text-red-600 hover:bg-red-50 rounded"
                    >
                      <Trash2 size={16} />
                    </button>
                  </div>
                </div>
                
                <div className="flex items-center gap-4 text-sm text-gray-500 mb-4">
                  <div className="flex items-center gap-1">
                    <Clock size={14} />
                    <span>{(recipe.cookingTime || 0)}m</span>
                  </div>
                </div>
                
                <div className="flex flex-wrap gap-1 mb-4">
                  {recipe.dietaryTags?.map(tag => (
                    <span key={tag} className="px-2 py-1 bg-blue-100 text-blue-800 text-xs rounded-full">
                      {tag}
                    </span>
                  ))}
                </div>
                
                <button
                  onClick={() => {
                    setSelectedRecipe(recipe);
                    setCurrentView('detail');
                  }}
                  className="w-full py-2 bg-gray-100 text-gray-700 rounded-md hover:bg-gray-200 transition-colors"
                >
                  View Recipe
                </button>
              </div>
            </div>
          ))}
        </div>
      )}

      {filteredRecipes.length === 0 && !loading && (
        <div className="text-center py-12">
          <ChefHat size={48} className="mx-auto text-gray-400 mb-4" />
          <p className="text-gray-600">No recipes found</p>
          {searchTerm && (
            <p className="text-sm text-gray-500 mt-2">
              Try adjusting your search terms
            </p>
          )}
        </div>
      )}
    </div>
  );

  const RecipeDetail = ({ recipe }) => (
    <div className="max-w-4xl mx-auto p-6">
      <div className="bg-white rounded-lg shadow-lg">
        <div className="p-6 border-b">
          <div className="flex justify-between items-start mb-4">
            <div>
              <h1 className="text-3xl font-bold text-gray-800 mb-2">{recipe.title}</h1>
            </div>
            <div className="flex gap-2">
              <button
                onClick={() => {
                  setEditingRecipe(recipe);
                  setCurrentView('form');
                  setErrors({});
                }}
                className="flex items-center gap-2 px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700"
              >
                <Edit size={16} />
                Edit
              </button>
              <button
                onClick={() => handleDelete(recipe.id)}
                className="flex items-center gap-2 px-4 py-2 bg-red-600 text-white rounded-md hover:bg-red-700"
              >
                <Trash2 size={16} />
                Delete
              </button>
            </div>
          </div>
          
          <div className="flex flex-wrap gap-2 mt-4">
            {recipe.dietaryTags?.map(tag => (
              <span key={tag} className="px-3 py-1 bg-blue-100 text-blue-800 text-sm rounded-full">
                {tag}
              </span>
            ))}
          </div>
        </div>
        
        <div className="p-6">
          <div className="grid grid-cols-1 lg:grid-cols-2 gap-8">
            <div>
              <h2 className="text-2xl font-semibold text-gray-800 mb-4">Ingredients</h2>
              <ul className="space-y-2">
                {recipe.ingredients.map((ingredient, index) => (
                  <li key={index} className="flex items-start gap-2">
                    <span className="w-2 h-2 bg-blue-600 rounded-full mt-2 flex-shrink-0"></span>
                    <span className="text-gray-700">{ingredient}</span>
                  </li>
                ))}
              </ul>
            </div>
            
            <div>
              <h2 className="text-2xl font-semibold text-gray-800 mb-4">Steps</h2>
              <ol className="space-y-4">
                {recipe.steps?.map((step, index) => (
                  <li key={index} className="flex gap-3">
                    <span className="flex-shrink-0 w-6 h-6 bg-blue-600 text-white rounded-full flex items-center justify-center text-sm font-medium">
                      {index + 1}
                    </span>
                    <span className="text-gray-700">{step}</span>
                  </li>
                ))}
              </ol>
            </div>
          </div>
        </div>
        
        <div className="p-6 border-t">
          <button
            onClick={() => setCurrentView('list')}
            className="px-6 py-2 bg-gray-100 text-gray-700 rounded-md hover:bg-gray-200"
          >
            ← Back to Recipes
          </button>
        </div>
      </div>
    </div>
  );

  return (
    <div className="min-h-screen bg-gray-50">
      {message && (
        <div className={`fixed top-4 right-4 z-50 px-4 py-2 border rounded-md shadow-lg ${
          message.includes('Failed') || message.includes('error') 
            ? 'bg-red-100 border-red-400 text-red-700' 
            : 'bg-green-100 border-green-400 text-green-700'
        }`}>
          {message}
        </div>
      )}
      
      {currentView === 'list' && <RecipeList />}
      {currentView === 'detail' && selectedRecipe && <RecipeDetail recipe={selectedRecipe} />}
      {currentView === 'form' && (
        <RecipeForm
          recipe={editingRecipe}
          onSave={handleSave}
          onCancel={() => {
            setCurrentView('list');
            setEditingRecipe(null);
            setErrors({});
          }}
        />
      )}
    </div>
  );
};

export default RecipeManager;