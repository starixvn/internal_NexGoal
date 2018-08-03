import React, { Component } from 'react';
import { Button, Form, FormGroup } from 'react-bootstrap';
import axios from 'axios';
import { toast } from 'react-toastify';
import Select from 'react-select';
import { camelCase, isEmpty } from 'lodash';
import { stringify } from 'qs';

import commonFunctions from '../../common-functions';
import EmployeeList from './EmployeeList';

class GroupFlowSetting extends Component {
    constructor(props) {
        super(props);

        this.state = {
            groups: [],
            isHost: document.getElementById('IsHost').checked
        };
    }

    componentWillMount() {
        this.getGroups();
    }

    getGroups() {
        axios(commonFunctions.getRootUrl() + 'Common/GetGroups')
            .then(response => {
                const data = response.data;

                this.setState({
                    groups: data
                });
            }).catch(commonFunctions.axiosError);
    }

    render() {
        return (
            <div className="row">
                <div className="col-sm-3">
                    <EmployeeList groupOptions={this.state.groups} />
                </div>
                <div className="col-sm-9 flow-settings">
                </div>
            </div>
        );
    }
}

export default GroupFlowSetting;