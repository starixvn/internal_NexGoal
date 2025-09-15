import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import axios from 'axios';
import { axiosError, resolveClientUrl } from '../../common-functions';

export default class Menu extends Component {
    constructor(props) {
        super(props);
        this.state = {
            menuItems: props.menuItems || [],
            hoverItem: null,
            currentUrl: '',
            isExpanded: false,
        };
    }

    componentDidMount() {
        this.getMenu();
        this.setCurrentUrl();
    }

    getMenu = () => {
        axios.get(resolveClientUrl('Menu/GetMenuItems')).then(response => {
            this.setState({
                menuItems: response.data
            });
        }).catch(axiosError);
    };

    setCurrentUrl = () => {
        const path = window.location.pathname.toLowerCase();
        const isHomeOrMyProbation = path === '/' || path === '/myprobation';
        const currentUrl = isHomeOrMyProbation ? '/home/index' : path;
        this.setState({ currentUrl });
    }

    onToggleMenu = () => {
        this.setState(prevState => {
            return { isExpanded: !prevState.isExpanded };
        });
    };

    handleMouseEnter = index => {
        this.setState({ hoverItem: index });
    };
    
    handleMouseLeave = () => {
        this.setState({ hoverItem: null });
    };

    isActive = link => this.state.currentUrl === link.toLowerCase() ? 'active' : '';

    render() {
        const { menuItems, hoverItem, currentUrl, isExpanded } = this.state

        return (
            <div className={`pull-left nav-hamberger ${isExpanded ? 'expended' : ''}`} id='js-navHamburger'>
                <div className='toggle-btn' id='js-btnToggleMenu' onClick={this.onToggleMenu}>
                    <span className='lines'></span>
                </div>
                { menuItems && menuItems.length > 0 && (
                    <ul className='nav navbar-nav'>
                        {
                            menuItems.map((menuGroup, index) => (
                                menuGroup.childs && menuGroup.childs.length > 0 ?
                                    <li key={menuGroup.name} className={`dropdown ${hoverItem === index ? 'open' : ''} ${
                                        menuGroup.childs.some(c => currentUrl === c.link.toLowerCase()) ? 'active' : ''}`}
                                        onMouseEnter={() => this.handleMouseEnter(index)}
                                        onMouseLeave={this.handleMouseLeave}
                                    >
                                        <a href='javascript:;' className='dropdown-toggle'>
                                            {menuGroup.name} <span className='icon-submenu'></span>
                                        </a>
                                        <ul className='dropdown-menu'>
                                            {
                                                menuGroup.childs.map(child =>
                                                    <li key={child.name} className={this.isActive(child.link)} >
                                                        <a href={child.link}>{child.name}</a>
                                                    </li>
                                                )
                                            }
                                        </ul>
                                    </li>
                                    :
                                    <li key={menuGroup.name} className={this.isActive(menuGroup.link)} >
                                        <a href={menuGroup.link}>{menuGroup.name}</a>
                                    </li>
                                )
                            )
                        }
                    </ul>
                )}
            </div>
        );
    }
}

ReactDOM.render(<Menu />, document.getElementById('menu-wrapper'));