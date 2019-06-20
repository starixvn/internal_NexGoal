import React, { Component } from 'react';
import axios from 'axios';

import { axiosError, resolveClientUrl } from '../../../common-functions';
import Table from './Table';
import CommentModal from './CommentModal';

export default class Probation extends Component {
    constructor(props) {
        super(props);

        this.state = {
            probationers: null,
            commentModal: {
                show: false,
                content: ''
            }
        };
    }

    componentDidMount() {
        const promises = [
            axios(resolveClientUrl('Probation/GetEvaluations')),
        ];

        Promise.all(promises)
            .then(([evaluationResponse]) => {
                this.setState({
                    probationers: evaluationResponse.data
                });
            }).catch(axiosError);
    }

    handleShowCommentModal = comment => this.setState({ commentModal: { show: true, content: comment } })

    handleCloseCommentModal = () => this.setState({ commentModal: { show: false } })

    render() {
        return (
            <div className="probation-content">
                <Table tableData={this.state.probationers} onShowComment={this.handleShowCommentModal} />
                <CommentModal showModal={this.state.commentModal.show} content={this.state.commentModal.content}
                    onCloseModal={this.handleCloseCommentModal}
                />
            </div>
        );
    }
}