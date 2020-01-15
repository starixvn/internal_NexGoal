import React, { Component } from 'react';
import ReactDOM from 'react-dom';

import { resolveClientUrl } from '../../../common-functions';
import EditLayout from './EditLayout';
import Settings from './Settings';
import RoleManagement from './role/index';

export default class Edit extends Component {
    constructor(props) {
        super(props);

        this.state = {
            activeKey: 1,
            isFirstLoad: false
        };
    }

    handleBackClick() {
        window.location = resolveClientUrl('NominationPeriod/index?tab=eoq');
    }

    handleSelect = key => {
        this.setState({ activeKey: key });

        if (key === 2 && !this.state.isFirstLoad) {
            this.setState({ isFirstLoad: true });
        }
    }

    render() {
        return (
            <EditLayout activeKey={this.state.activeKey}
                isFirstLoad={this.state.isFirstLoad}
                settings={<Settings />}
                roleManagement={<RoleManagement />}
                onBack={this.handleBackClick}
                onSelect={this.handleSelect}
            />
        );
    }
}

ReactDOM.render(<Edit />, document.getElementById('nomination-wrapper'));