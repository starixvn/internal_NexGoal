import React, { Component, Fragment } from 'react';
import ReactDOM from 'react-dom';
import axios from 'axios';
import { stringify } from 'qs';
import commonFunctions from '../../../common-functions';
import { toast } from 'react-toastify';
import { camelCase, range } from 'lodash';
import SearchForm from './SearchForm'

export default class EditEmployeeSetting extends Component {
    constructor(props) {
        super(props);
    }

    render() {
        return (
            <Fragment>
                <SearchForm />
            </Fragment>

        );
    }
}