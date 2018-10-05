import React, { Component } from "react";
import axios from 'axios';
import { axiosError, resolveClientUrl } from '../../common-functions';

export default class EvaluatorTimeline extends Component {
    constructor(props) {
        super(props);

        this.state = {
            memberPCIs: []
        };
    }

    getMembersPCI = function () {
        axios.get(resolveClientUrl('Self/Performance/Index'))
            .then(response => {
                this.setState(
                    {
                        memberPCIs: response.data.memberPCIs
                    });
            }).catch(axiosError);
    }
}